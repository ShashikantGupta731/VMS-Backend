using backend.DTOs;
using backend.DTOs.Masters;

namespace backend.Services
{
    public interface IVehicleService
    {
        Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto, int userId);
        Task<List<VehicleResponseDto>> GetAllVehiclesAsync(int? status = null);
        Task<VehicleResponseDto?> GetVehicleByIdAsync(int id);
        Task<List<VehicleResponseDto>> GetVehiclesByDdoAsync(string ddoCode);
        Task<VehicleResponseDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto);
        Task<bool> DeleteVehicleAsync(int id);
        Task<bool> VerifyVehicleAsync(int id, string verifierId, string comments, int status);
        Task<bool> TransferVehicleAsync(TransferVehicleDto dto, int userId);
        Task<bool> CondemnVehicleAsync(CondemnVehicleDto dto, int userId);
    }
}
