using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models.Masters;

namespace backend.Models.Core
{
    public class FuelBill
    {
        [Key]
        public int FuelBillId { get; set; }

        [Required]
        public int VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public virtual VehicleInfo Vehicle { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string BillNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        public int OdometerReading { get; set; }

        [Required]
        public decimal FuelQuantity { get; set; } // Litres

        [Required]
        public decimal Amount { get; set; }

        public string? NocFile { get; set; }
        public DateTime? NocIssueDate { get; set; }
        public DateTime? NocExpiryDate { get; set; }
        public string? SanctionPermissionFile { get; set; }

        [Required]
        public BillStatus Status { get; set; } = BillStatus.Draft;

        public int? ClaimId { get; set; }
        [ForeignKey("ClaimId")]
        public virtual BillClaim? Claim { get; set; }

        [Required]
        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
