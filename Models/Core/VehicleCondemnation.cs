using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class VehicleCondemnation
    {
        [Key]
        public int VehicleCondemnationId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual VehicleInfo Vehicle { get; set; } = null!;

        [Required]
        public DateTime CondemnationDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CondemnationOrderNumber { get; set; } = string.Empty;

        public string CondemnationOrderPath { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        [StringLength(20)]
        public string AuctionStatus { get; set; } = string.Empty; // Pending, Completed

        public DateTime? AuctionDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AuctionAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }
    }
}
