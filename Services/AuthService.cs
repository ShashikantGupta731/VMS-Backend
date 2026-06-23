using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Core;

namespace backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly OtpService _otpService;
        private readonly IIfmsService _ifmsService;

        public AuthService(AppDbContext context, JwtService jwtService, OtpService otpService, IIfmsService ifmsService)
        {
            _context = context;
            _jwtService = jwtService;
            _otpService = otpService;
            _ifmsService = ifmsService;
        }

        // Login user
        public async Task<(bool Success, string? Token, object? User, string? Message)> LoginAsync(string username, string password)
        {
            Console.WriteLine($"5. AuthService.LoginAsync: Started for User='{username}'");
            
            // Find user by username
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                Console.WriteLine($"6. AuthService.LoginAsync: User '{username}' not found locally. Attempting IFMS Fallback...");
                
                var (ifmsSuccess, ifmsData, ifmsMessage) = await _ifmsService.LoginViaIfmsAsync(username, password);
                
                if (ifmsSuccess && ifmsData != null)
                {
                    Console.WriteLine($"6a. AuthService.LoginAsync: IFMS Login SUCCESS for '{username}'. Mapping to local user '{ifmsData.ddoCode}'...");
                    
                    // Legacy logic: Map IFMS user to local user via DDOCode, or fallback to username for admin users
                    user = await _context.Users
                        .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.Username == ifmsData.ddoCode || u.DDOCode == ifmsData.ddoCode || (ifmsData.ddoCode == null && u.Username == "admin"));

                    if (user == null)
                    {
                        Console.WriteLine($"6b. AuthService.LoginAsync: ABORTED - IFMS User '{ifmsData.ddoCode}' not mapped in local database.");
                        return (false, null, null, "Your IFMS account is valid, but not authorized for VMS. Please contact Administrator.");
                    }
                }
                else
                {
                    Console.WriteLine($"6a. AuthService.LoginAsync: ABORTED - Local user not found and IFMS fallback failed: {ifmsMessage}");
                    return (false, null, null, "Invalid username or password");
                }
            }
            else
            {
                // Local user exists, verify password
                Console.WriteLine("7. AuthService.LoginAsync: User found locally. Calling PasswordHasher.VerifyPassword...");
                if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    Console.WriteLine("11. AuthService.LoginAsync: Password verification FAILED.");
                    // Increment failed attempts
                    user.FailedAttempts++;
                    
                    // Lock account after 5 failed attempts
                    if (user.FailedAttempts >= 5)
                    {
                        user.LockUntil = DateTime.UtcNow.AddMinutes(15); // Lock for 15 minutes
                        await _context.SaveChangesAsync();
                        Console.WriteLine("11. AuthService.LoginAsync: LIMIT REACHED - Account locked for 15 mins.");
                        return (false, null, null, "Account locked due to too many failed attempts. Try again in 15 minutes.");
                    }

                    await _context.SaveChangesAsync();
                    return (false, null, null, "Invalid username or password");
                }
            }

            Console.WriteLine($"7. AuthService.LoginAsync: User Authenticated. ID={user.UserId}, Enabled={user.Enabled}, LockedUntil={user.LockUntil}");

            // Check if account is locked
            if (user.LockUntil.HasValue && user.LockUntil > DateTime.UtcNow)
            {
                Console.WriteLine($"8. AuthService.LoginAsync: ABORTED - Account locked until {user.LockUntil}");
                return (false, null, null, $"Account locked. Try again after {user.LockUntil:yyyy-MM-dd HH:mm}");
            }

            // Check if account is active
            if (!user.Enabled)
            {
                Console.WriteLine("9. AuthService.LoginAsync: ABORTED - Account is DEACTIVATED");
                return (false, null, null, "Account is deactivated");
            }

            Console.WriteLine("11. AuthService.LoginAsync: Password verification SUCCESS.");

            // Reset failed attempts on successful login
            user.FailedAttempts = 0;
            user.LockUntil = null;
            await _context.SaveChangesAsync();

            // Get user roles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            Console.WriteLine($"12. AuthService.LoginAsync: Roles identified: {string.Join(",", roles)}");

            // Generate JWT token
            Console.WriteLine("13. AuthService.LoginAsync: Calling JwtService.GenerateToken...");
            var token = _jwtService.GenerateToken(user, roles);

            // Return user info and token
            var userData = new
            {
                id = user.UserId,
                username = user.Username,
                name = user.Name,
                phone = user.PhoneNo,
                roles,
                isGuest = user.IsGuest,
                ddoCode = user.DDOCode   // IFMS-formatted DDO code (e.g. CHD00/0135) — used in Treasury bill submission
            };

            Console.WriteLine("14. AuthService.LoginAsync: Login flow COMPLETE.");
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
                .FirstOrDefaultAsync(u => u.PhoneNo == phone);
            
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
                PhoneNo = phone,
                Enabled = true,
                IsGuest = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign default "User" role (you need to create this role in the database first)
            // For now, we'll skip role assignment if the role doesn't exist
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "GUEST");
            if (userRole != null)
            {
                var userRoleMapping = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = userRole.RoleId
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

            // TODO: Integrate real SMS Gateway service (using legacy template IDs)
            Console.WriteLine($"GUEST OTP GENERATED for {phone}: {otp}"); 
            
            return (true, otp, "OTP sent successfully"); // Don't return OTP in production
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
                .FirstOrDefaultAsync(u => u.PhoneNo == phone && u.IsGuest);

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
                    PhoneNo = phone,
                    IsGuest = true,
                    Enabled = true,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Assign default guest role
            var roles = new List<string> { "GUEST" };

            // Generate JWT token
            var token = _jwtService.GenerateToken(user, roles);

            var userData = new
            {
                id = user.UserId,
                username = user.Username,
                name = user.Name,
                phone = user.PhoneNo,
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNo == phone && !u.IsGuest);

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

                // Change password
        public async Task<(bool Success, string? Message)> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            Console.WriteLine($"AuthService.ChangePasswordAsync: Started for UserId='{userId}'");

            // 1. Find user by ID including their password history
            var user = await _context.Users
                .Include(u => u.PasswordHistories)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return (false, "User not found.");
            }

            // 2. Verify Old Password
            if (!PasswordHasher.VerifyPassword(oldPassword, user.PasswordHash))
            {
                return (false, "Old Password is incorrect.");
            }

            // 3. Ensure new password is not the same as the current password
            if (PasswordHasher.VerifyPassword(newPassword, user.PasswordHash))
            {
                return (false, "New password cannot be the same as your current password.");
            }

            // 4. Check Password History (Policy: Cannot reuse last 5 passwords)
            int passwordPolicyLimit = 5; 
            var recentPasswords = user.PasswordHistories
                                      .OrderByDescending(ph => ph.CreatedAt)
                                      .Take(passwordPolicyLimit)
                                      .ToList();

            foreach (var history in recentPasswords)
            {
                if (PasswordHasher.VerifyPassword(newPassword, history.PasswordHash))
                {
                    return (false, $"You cannot repeat your last {passwordPolicyLimit} passwords.");
                }
            }

            // 5. Hash the new password
            var newPasswordHash = PasswordHasher.HashPassword(newPassword);

            // 6. Update user's current password
            user.PasswordHash = newPasswordHash;

            // 7. Add to Password History
            var newHistoryRecord = new PasswordHistory
            {
                UserId = user.UserId,
                PasswordHash = newPasswordHash,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.PasswordHistories.Add(newHistoryRecord);

            // 8. Save changes to DB
            await _context.SaveChangesAsync();

            Console.WriteLine("AuthService.ChangePasswordAsync: Password successfully changed.");
            return (true, "Password changed successfully.");
        }

    }
}
