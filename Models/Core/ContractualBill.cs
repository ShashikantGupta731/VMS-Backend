using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class ContractualBill
    {
        [Key]
        public int ContractualBillId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        public DateTime BillPeriodFrom { get; set; }

        [Required]
        public DateTime BillPeriodTo { get; set; }

        [Required]
        [MaxLength(20)]
        public string DdoCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string VehicleNumber { get; set; } = string.Empty;

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
