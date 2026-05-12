using backend.Data;
using backend.DTOs.User;
using backend.Models.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserResponseDto>> GetUsersAsync(int userId, string role, int? departmentId, int? districtId)
        {
            var query = _context.Users
                .Include(u => u.Department)
                .Include(u => u.District)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            // Role-based data isolation
            if (role == "HOD" && departmentId.HasValue)
            {
                query = query.Where(u => u.DeptId == departmentId.Value);
            }
            else if (role == "DCL" && districtId.HasValue)
            {
                query = query.Where(u => u.DistrictId == districtId.Value);
            }
            // ADMN sees all. If unknown role, maybe return empty or just their own.
            else if (role != "ADMN")
            {
                query = query.Where(u => u.UserId == userId);
            }

            var users = await query.ToListAsync();
            return users.Select(MapToResponseDto).ToList();
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            // Uniqueness Check
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u.EmailId == dto.Email))
            {
                throw new Exception("Username or Email already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                Name = dto.Name,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                EmailId = dto.Email,                 // Updated to use new field name
                PhoneNo = dto.Phone,                 // Updated to use new field name
                DistrictId = dto.DistrictId > 0 ? dto.DistrictId : null,
                DeptId = dto.DepartmentId > 0 ? dto.DepartmentId : null,  // Updated to use new field name
                DDOCode = dto.DDOCode,
                DDORegistrationNo = dto.DDORegistrationNo,
                ManagedDdos = dto.ManagedDdos,
                Enabled = dto.IsActive,                 // Updated to use new field name
                IsNonTreasuryDDO = dto.IsNonTreasuryDDO
            };

            // Hash password
            user.PasswordHash = PasswordHasher.HashPassword(dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Handle roles
            foreach (var roleName in dto.Roles)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = role.RoleId });
                }
            }
            await _context.SaveChangesAsync();

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return null;

            user.Username = dto.Username;
            user.Name = dto.Name;
            user.FirstName = dto.FirstName;
            user.MiddleName = dto.MiddleName;
            user.LastName = dto.LastName;
            user.EmailId = dto.Email;
            user.PhoneNo = dto.Phone;
            user.DistrictId = dto.DistrictId > 0 ? dto.DistrictId : null;
            user.DeptId = dto.DepartmentId > 0 ? dto.DepartmentId : null;
            user.DDOCode = dto.DDOCode;
            user.DDORegistrationNo = dto.DDORegistrationNo;
            user.ManagedDdos = dto.ManagedDdos;
            user.Enabled = dto.IsActive;
            user.IsNonTreasuryDDO = dto.IsNonTreasuryDDO;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = PasswordHasher.HashPassword(dto.Password);
            }

            // Update roles
            _context.UserRoles.RemoveRange(user.UserRoles);
            foreach (var roleName in dto.Roles)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = role.RoleId });
                }
            }

            await _context.SaveChangesAsync();

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.District)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            return user == null ? null : MapToResponseDto(user);
        }

        public async Task<List<DdoResponseDto>> GetAllDdoInformationAsync(int? deptId, int? districtId, int? userId = null, string? role = null)
        {
            var query = _context.Users.AsQueryable();

            if (deptId.HasValue && deptId.Value > 0)
            {
                query = query.Where(u => u.DeptId == deptId.Value);
            }

            if (districtId.HasValue && districtId.Value > 0)
            {
                query = query.Where(u => u.DistrictId == districtId.Value);
            }

            return await query
                .Where(u => !string.IsNullOrEmpty(u.DDOCode))
                .Select(u => new DdoResponseDto
                {
                    Id = u.UserId,
                    DdoCode = u.DDOCode
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> CheckUsernameAvailabilityAsync(string username)
        {
            return !await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> AdminResetPasswordAsync(int id, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return false;

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return false;

            user.Enabled = !user.Enabled;
            await _context.SaveChangesAsync();
            return true;
        }

        private UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.UserId,                    // Updated to use new field name
                Username = user.Username,
                Name = user.Name,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.EmailId,                 // Updated to use new field name
                Phone = user.PhoneNo ?? string.Empty,                 // Updated to use new field name
                DistrictId = user.DistrictId,
                DistrictName = user.District?.DistrictName ?? string.Empty,
                DepartmentId = user.DeptId,                 // Updated to use new field name
                DepartmentName = user.Department?.DeptName ?? string.Empty,
                DDOCode = user.DDOCode,
                DDORegistrationNo = user.DDORegistrationNo,
                ManagedDdos = user.ManagedDdos,
                IsActive = user.Enabled,                 // Updated to use new field name
                IsNonTreasuryDDO = user.IsNonTreasuryDDO,
                CreatedAt = user.CreatedDate,               // Updated to use new field name
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }
    }
}
