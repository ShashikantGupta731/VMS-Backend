namespace backend.Models.Masters
{
    public class Manufacturer
    {
        // Primary Key - matching legacy ManufacturerId
        public int ManufacturerId { get; set; }
        
        // Legacy-compatible field names
        public string ManufacturerName { get; set; } = string.Empty;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Status field - keeping IsActive for now
        public bool IsActive { get; set; } = true;
    }
}
