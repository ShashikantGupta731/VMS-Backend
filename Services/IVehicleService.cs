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
        Task<bool> UpdateDriverDetailsAsync(int id, UpdateDriverDetailsDto dto);
        Task<bool> DeleteVehicleAsync(int id);
        Task<bool> VerifyVehicleAsync(int id, string verifierId, string comments, int status);
        Task<bool> TransferVehicleAsync(TransferVehicleDto dto, int userId);
        Task<bool> RegisterReplacementVehicleAsync(RegisterReplacementVehicleDto dto, int userId);
        Task<bool> MarkForCondemnedAsync(MarkForCondemnedDto dto, int userId);
        Task<bool> RejectCondemnationAsync(string vehicleNumber, string reason, int userId);
        Task<bool> AddVehicleGrnNumberAndDetailsAsync(VehicleGrnDetailsDto dto, int userId);
        Task<List<backend.DTOs.Vehicle.FitnessCertificateDto>> GetFitnessCertificatesAsync(int vehicleId);
        Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetFuelBillsAsync(int vehicleId);
        Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetMaintenanceBillsAsync(int vehicleId);
        Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetServiceBillsAsync(int vehicleId);
        Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetBatteryChangesAsync(int vehicleId);
        Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetTyreChangesAsync(int vehicleId);
        Task<List<backend.DTOs.Vehicle.TransferHistoryDto>> GetTransferHistoryAsync(int vehicleId);
    }
}
