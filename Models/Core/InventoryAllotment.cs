using System.ComponentModel.DataAnnotations;

namespace backend.Models.Core
{
    public class InventoryAllotment
    {
        public int InventoryAllotmentId { get; set; }
        
        [Required]
        public int InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }
        
        [Required]
        public int VehicleId { get; set; }
        public VehicleInfo? Vehicle { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public int AllottedByUserId { get; set; } // DDO who allotted
        public User? AllottedByUser { get; set; }
        
        public string? OdometerReading { get; set; }
        
        public string? Remarks { get; set; }
        
        public DateTime AllotmentDate { get; set; } = DateTime.UtcNow;
    }
}
