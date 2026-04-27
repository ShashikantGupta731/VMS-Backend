using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly OtpService _otpService;

        public AuthService(AppDbContext context, JwtService jwtService, OtpService otpService)
        {
            _context = context;
            _jwtService = jwtService;
            _otpService = otpService;
        }

        // Login user
        public async Task<(bool Success, string? Token, object? User, string? Message)> LoginAsync(string username, string password)
        {
            // Find user by username
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return (false, null, null, "Invalid username or password");
            }

            // Check if account is locked
            if (user.LockUntil.HasValue && user.LockUntil > DateTime.UtcNow)
            {
                return (false, null, null, $"Account locked. Try again after {user.LockUntil:yyyy-MM-dd HH:mm}");
            }

            // Check if account is active
            if (!user.IsActive)
            {
                return (false, null, null, "Account is deactivated");
            }

            // Verify password
            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                // Increment failed attempts
                user.FailedAttempts++;
                
                // Lock account after 5 failed attempts
                if (user.FailedAttempts >= 5)
                {
                    user.LockUntil = DateTime.UtcNow.AddMinutes(15); // Lock for 15 minutes
                    await _context.SaveChangesAsync();
                    return (false, null, null, "Account locked due to too many failed attempts. Try again in 15 minutes.");
                }

                await _context.SaveChangesAsync();
                return (false, null, null, "Invalid username or password");
            }

            // Reset failed attempts on successful login
            user.FailedAttempts = 0;
            user.LockUntil = null;
            await _context.SaveChangesAsync();

            // Get user roles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // Generate JWT token
            var token = _jwtService.GenerateToken(user, roles);

            // Return user info and token
            var userData = new
            {
                id = user.Id,
                username = user.Username,
                name = user.Name,
                phone = user.Phone,
                roles,
                isGuest = user.IsGuest
            };

            return (true, token, userData, null);
        }

        // Signup new user
        public async Task<(bool Success, string? Message)> SignupAsync(string username, string password, string name, string phone)
        {
            // Check if username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            
            if (existingUser != null)
            {
                return (false, "Username already exists");
            }

            // Check if phone already exists
            var existingPhone = await _context.Users
                .FirstOrDefaultAsync(u => u.Phone == phone);
            
            if (existingPhone != null)
            {
                return (false, "Phone number already registered");
            }

            // Hash password
            var passwordHash = PasswordHasher.HashPassword(password);

            // Create new user
            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Name = name,
                Phone = phone,
                IsActive = true,
                IsGuest = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign default "User" role (you need to create this role in the database first)
            // For now, we'll skip role assignment if the role doesn't exist
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole != null)
            {
                var userRoleMapping = new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id
                };
                _context.UserRoles.Add(userRoleMapping);
                await _context.SaveChangesAsync();
            }

            return (true, "Account created successfully. Please login.");
        }

        // Guest login - generate OTP
        public (bool Success, string? Otp, string? Message) GuestLogin(string name, string phone)
        {
            // Generate OTP
            var otp = _otpService.GenerateOtp();
            
            // Store OTP with phone number and name
            _otpService.StoreOtp(phone, otp, name);

            // TODO: Send OTP via SMS (for now, return in response for testing)
            return (true, otp, "OTP sent successfully");
        }

        // Verify OTP and create/retrieve guest user
        public async Task<(bool Success, string? Token, object? User, string? Message)> VerifyOtpAsync(string phone, string otp)
        {
            // Verify OTP and get name
            var (isValid, name) = _otpService.VerifyOtp(phone, otp);
            
            if (!isValid)
            {
                return (false, null, null, "Invalid or expired OTP");
            }

            // Check if guest user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Phone == phone && u.IsGuest);

            User user;

            if (existingUser != null)
            {
                user = existingUser;
            }
            else
            {
                // Create new guest user
                user = new User
                {
                    Username = $"guest_{phone}",
                    PasswordHash = "", // No password for guest
                    Name = name,
                    Phone = phone,
                    IsGuest = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Assign default guest role (you can create a "Guest" role in the database)
            // For now, return empty roles
            var roles = new List<string>();

            // Generate JWT token
            var token = _jwtService.GenerateToken(user, roles);

            var userData = new
            {
                id = user.Id,
                username = user.Username,
                name = user.Name,
                phone = user.Phone,
                roles,
                isGuest = user.IsGuest
            };

            return (true, token, userData, null);
        }

        // Reset password with OTP
        public async Task<(bool Success, string? Message)> ResetPasswordAsync(string phone, string otp, string newPassword)
        {
            // Verify OTP
            var (isValid, _) = _otpService.VerifyOtp(phone, otp);
            
            if (!isValid)
            {
                return (false, "Invalid or expired OTP");
            }

            // Find user by phone
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone && !u.IsGuest);

            if (user == null)
            {
                return (false, "User not found");
            }

            // Update password
            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            user.FailedAttempts = 0;
            user.LockUntil = null;
            await _context.SaveChangesAsync();

            return (true, "Password reset successfully");
        }
    }
}
