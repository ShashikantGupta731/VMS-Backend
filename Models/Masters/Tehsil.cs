namespace backend.Models.Masters
{
    public class Tehsil
    {
        // Primary Key - matching legacy TehsilId
        public int TehsilId { get; set; }
        
        // Legacy-compatible field names
        public string TehsilName { get; set; } = string.Empty;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Foreign Key to District
        public int DistrictId { get; set; }
        public District District { get; set; } = null!;
        
        // Status field - keeping IsActive for now
        public bool IsActive { get; set; } = true;
    }
}
