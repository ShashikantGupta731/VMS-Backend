using backend.DTOs;

namespace backend.Services
{
    public interface IVehicleService
    {
        Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto, int userId);
        Task<List<VehicleResponseDto>> GetAllVehiclesAsync();
        Task<VehicleResponseDto?> GetVehicleByIdAsync(int id);
        Task<VehicleResponseDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto);
        Task<bool> DeleteVehicleAsync(int id);
    }
}
