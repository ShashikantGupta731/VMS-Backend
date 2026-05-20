using backend.DTOs.Billing;
using backend.Models.Core;

namespace backend.Services
{
    public interface IBillingService
    {
        // Fuel Bills
        Task<List<FuelBillResponseDto>> GetFuelBillsAsync(int userId, int? claimId = null);
        Task<FuelBillResponseDto?> GetFuelBillByIdAsync(int id, int userId);
        Task<FuelBillResponseDto> SaveFuelBillAsync(CreateFuelBillDto dto, int userId);
        Task<bool> DeleteFuelBillAsync(int id, int userId);

        // Maintenance Bills
        Task<List<MaintenanceBillResponseDto>> GetMaintenanceBillsAsync(int userId, int? claimId = null);
        Task<MaintenanceBillResponseDto?> GetMaintenanceBillByIdAsync(int id, int userId);
        Task<MaintenanceBillResponseDto> SaveMaintenanceBillAsync(CreateMaintenanceBillDto dto, int userId);
        Task<bool> DeleteMaintenanceBillAsync(int id, int userId);

        // Hired Vehicle Bills
        Task<List<HiredVehicleBillResponseDto>> GetHiredVehicleBillsAsync(int userId, int? claimId = null);
        Task<HiredVehicleBillResponseDto> SaveHiredVehicleBillAsync(CreateHiredVehicleBillDto dto, int userId);
        Task<bool> DeleteHiredVehicleBillAsync(int id, int userId);

        // Contractual Bills
        Task<List<ContractualBillResponseDto>> GetContractualBillsAsync(int userId, int? claimId = null);
        Task<ContractualBillResponseDto> SaveContractualBillAsync(CreateContractualBillDto dto, int userId);
        Task<bool> DeleteContractualBillAsync(int id, int userId);

        // Miscellaneous Bills
        Task<List<MiscellaneousBillResponseDto>> GetMiscellaneousBillsAsync(int userId, int? claimId = null);
        Task<MiscellaneousBillResponseDto> SaveMiscellaneousBillAsync(CreateMiscellaneousBillDto dto, int userId);
        Task<bool> DeleteMiscellaneousBillAsync(int id, int userId);

        // Claims
        Task<BillClaimResponseDto> CreateClaimAsync(CreateClaimDto dto, int userId);
        Task<List<BillClaimResponseDto>> GetClaimsAsync(int userId, string role, BillStatus? status = null, bool? forwardedToTreasury = null);
        Task<BillClaimDetailDto?> GetClaimByIdAsync(int id);
        Task<bool> VerifyClaimAsync(int claimId, int verifierId, string? comments);
        Task<bool> RejectClaimAsync(int claimId, int verifierId, string comments);

        // Validation Helpers
        Task<OdometerValidationDto> GetOdometerValidationAsync(int vehicleId, DateTime billDate);
        Task<bool> CheckFitnessRequirementAsync(int vehicleId, int currentOdometer);
        Task<bool> CheckPermissionRequirementAsync(int vehicleId, BillType type, decimal amountOrLitres);

        // Personal Usage
        Task<bool> InsertPersonalUseDetailsAsync(PersonalUsagePayloadDto dto);
    }
}
