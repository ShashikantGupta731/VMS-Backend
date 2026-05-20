using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class MiscellaneousBill
    {
        [Key]
        public int MiscellaneousBillId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        public int InventoryItemId { get; set; }
        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem InventoryItem { get; set; } = null!;

        public string? ModelNumber { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public BillStatus Status { get; set; } = BillStatus.Draft;

        public int? ClaimId { get; set; }
        [ForeignKey("ClaimId")]
        public virtual BillClaim? Claim { get; set; }

        // Audit Fields
        [Required]
        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
