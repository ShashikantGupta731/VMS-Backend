using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models.Core;

namespace backend.Models.Core
{
    public enum BillStatus
    {
        Draft = 0,
        Pending = 1,
        Verified = 2,
        Rejected = 3
    }

    public enum BillType
    {
        Fuel = 1,
        Maintenance = 2,
        Hired = 3,
        Contractual = 4,
        Miscellaneous = 5
    }

    public class BillClaim
    {
        [Key]
        public int BillClaimId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClaimNumber { get; set; } = string.Empty;

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public BillStatus Status { get; set; } = BillStatus.Pending;

        [Required]
        public BillType Type { get; set; }

        public int? VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public virtual VehicleInfo? Vehicle { get; set; }

        public string? Comments { get; set; }

        // Sanction & Voucher Details
        public bool ForwardedToTreasury { get; set; }
        public string? SubVoucherNo { get; set; }
        public string? SubVoucherDescription { get; set; }
        public string? SanctionOrderNo { get; set; }
        public DateTime? SanctionOrderDate { get; set; }
        public string? SanctionAuthority { get; set; }
        public string? FirmName { get; set; }
        public decimal Tax { get; set; }

        // Audit Fields
        [Required]
        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; } = null!;

        public int? VerifiedById { get; set; }
        [ForeignKey("VerifiedById")]
        public virtual User? VerifiedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? VerifiedAt { get; set; }

        // Navigation properties
        public virtual ICollection<FuelBill> FuelBills { get; set; } = new List<FuelBill>();
        public virtual ICollection<MaintenanceBill> MaintenanceBills { get; set; } = new List<MaintenanceBill>();
        public virtual ICollection<HiredVehicleBill> HiredVehicleBills { get; set; } = new List<HiredVehicleBill>();
        public virtual ICollection<ContractualBill> ContractualBills { get; set; } = new List<ContractualBill>();
        public virtual ICollection<MiscellaneousBill> MiscellaneousBills { get; set; } = new List<MiscellaneousBill>();
    }
}
