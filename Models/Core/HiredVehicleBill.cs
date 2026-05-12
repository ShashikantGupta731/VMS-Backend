using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class HiredVehicleBill
    {
        [Key]
        public int HiredVehicleBillId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string OfficeName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContractorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string ContractorPhone { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        public int NoOfVehicles { get; set; } = 1;

        [Required]
        public DateTime HiredFrom { get; set; }

        [Required]
        public DateTime HiredTo { get; set; }

        [Required]
        public int KmCovered { get; set; }

        [Required]
        public decimal Amount { get; set; }

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
