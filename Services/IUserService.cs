using backend.DTOs.User;

namespace backend.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetUsersAsync(int userId, string role, int? departmentId, int? districtId);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
        Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<List<DdoResponseDto>> GetAllDdoInformationAsync(int? deptId, int? districtId, int? userId = null, string? role = null);
        Task<bool> CheckUsernameAvailabilityAsync(string username);
        Task<bool> AdminResetPasswordAsync(int id, string newPassword);
        Task<bool> ToggleUserStatusAsync(int id);
    }

    public class DdoResponseDto
    {
        public int Id { get; set; }
        public string DdoCode { get; set; } = string.Empty;
    }
}
