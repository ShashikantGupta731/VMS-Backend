using System.ComponentModel.DataAnnotations;

namespace backend.Models.Core
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., Tyre, Battery, Engine Oil
        
        public string? Category { get; set; } // e.g., Spare Part, Consumable
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
