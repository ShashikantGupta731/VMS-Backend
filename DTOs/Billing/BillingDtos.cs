using backend.Models.Core;

namespace backend.DTOs.Billing
{
    // Fuel Bill DTOs
    public class CreateFuelBillDto
    {
        public int? FuelBillId { get; set; } // For updates
        public int VehicleId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public int OdometerReading { get; set; }
        public decimal FuelQuantity { get; set; }
        public decimal Amount { get; set; }
        public string? NocFile { get; set; }
        public DateTime? NocIssueDate { get; set; }
        public DateTime? NocExpiryDate { get; set; }
        public string? SanctionPermissionFile { get; set; }
        public string? SanctionAuthorityMobileNo { get; set; }
    }

    public class FuelBillResponseDto
    {
        public int FuelBillId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public int OdometerReading { get; set; }
        public decimal FuelQuantity { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
        public string? ClaimNumber { get; set; }
        public string? SanctionAuthorityMobileNo { get; set; }
        public string? NocFile { get; set; }
        public DateTime? NocIssueDate { get; set; }
        public DateTime? NocExpiryDate { get; set; }
        public string? SanctionPermissionFile { get; set; }
    }

    // Maintenance Bill DTOs
    public class CreateMaintenanceBillDto
    {
        public int? MaintenanceBillId { get; set; }
        public int VehicleId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public int OdometerReading { get; set; }
        public decimal Amount { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? SanctionPermissionFile { get; set; }
    }

    public class MaintenanceBillResponseDto
    {
        public int MaintenanceBillId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public int OdometerReading { get; set; }
        public decimal Amount { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? SanctionPermissionFile { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
    }

    // Hired Vehicle Bill DTOs
    public class CreateHiredVehicleBillDto
    {
        public int? HiredVehicleBillId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string OfficeName { get; set; } = string.Empty;
        public string ContractorName { get; set; } = string.Empty;
        public string ContractorPhone { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public int NoOfVehicles { get; set; }
        public DateTime HiredFrom { get; set; }
        public DateTime HiredTo { get; set; }
        public int KmCovered { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
    }

    public class HiredVehicleBillResponseDto
    {
        public int HiredVehicleBillId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string OfficeName { get; set; } = string.Empty;
        public string ContractorName { get; set; } = string.Empty;
        public string ContractorPhone { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public int NoOfVehicles { get; set; }
        public DateTime HiredFrom { get; set; }
        public DateTime HiredTo { get; set; }
        public int KmCovered { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
    }

    // Contractual Bill DTOs
    public class CreateContractualBillDto
    {
        public int? ContractualBillId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public DateTime BillPeriodFrom { get; set; }
        public DateTime BillPeriodTo { get; set; }
        public string DdoCode { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
    }

    public class ContractualBillResponseDto
    {
        public int ContractualBillId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public DateTime BillPeriodFrom { get; set; }
        public DateTime BillPeriodTo { get; set; }
        public string DdoCode { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
    }

    // Miscellaneous Bill DTOs
    public class CreateMiscellaneousBillDto
    {
        public int? MiscellaneousBillId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public int InventoryMasterId { get; set; }
        public string? ModelNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
    }

    public class MiscellaneousBillResponseDto
    {
        public int MiscellaneousBillId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public int InventoryMasterId { get; set; }
        public string InventoryName { get; set; } = string.Empty;
        public string? ModelNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public int? ClaimId { get; set; }
    }

    // Bill Claim DTOs
    public class BillClaimResponseDto
    {
        public int BillClaimId { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public BillStatus Status { get; set; }
        public BillType Type { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Comments { get; set; }
        public bool ForwardedToTreasury { get; set; }
    }

    public class BillClaimDetailDto : BillClaimResponseDto
    {
        public string? SubVoucherNo { get; set; }
        public string? SubVoucherDescription { get; set; }
        public string? SanctionOrderNo { get; set; }
        public DateTime? SanctionOrderDate { get; set; }
        public string? SanctionAuthority { get; set; }
        public string? FirmName { get; set; }
        public decimal Tax { get; set; }

        public List<FuelBillResponseDto> FuelBills { get; set; } = new();
        public List<MaintenanceBillResponseDto> MaintenanceBills { get; set; } = new();
        public List<HiredVehicleBillResponseDto> HiredVehicleBills { get; set; } = new();
        public List<ContractualBillResponseDto> ContractualBills { get; set; } = new();
        public List<MiscellaneousBillResponseDto> MiscellaneousBills { get; set; } = new();
    }

    public class CreateClaimDto
    {
        public BillType Type { get; set; }
        public List<int> BillIds { get; set; } = new();
        
        // Sanction & Voucher Details
        public bool ForwardedToTreasury { get; set; }
        public string? SubVoucherNo { get; set; }
        public string? SubVoucherDescription { get; set; }
        public string? SanctionOrderNo { get; set; }
        public DateTime? SanctionOrderDate { get; set; }
        public string? SanctionAuthority { get; set; }
        public string? FirmName { get; set; }
        public decimal Tax { get; set; }
    }
}
