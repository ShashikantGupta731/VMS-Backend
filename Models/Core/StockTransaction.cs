using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class StockTransaction
    {
        public int StockTransactionId { get; set; }
        
        [Required]
        public int InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }
        
        [Required]
        public int UserId { get; set; } // The DDO who owns the stock
        public User? User { get; set; }
        
        [Required]
        public int Quantity { get; set; } // Positive for Addition, Negative for Allotment (if handled here)
        
        public string? BillNumber { get; set; }
        public DateTime? BillDate { get; set; }
        
        public string? Remarks { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
