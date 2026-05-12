namespace backend.Models.Masters
{
    public class District
    {
        // Primary Key - matching legacy DistrictId
        public int DistrictId { get; set; }
        
        // Legacy-compatible field names
        public string DistrictName { get; set; } = string.Empty;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Legacy abbreviation field
        public string DistAbbre { get; set; } = string.Empty;
        
        // Status field - keeping IsActive for now
        public bool IsActive { get; set; } = true;
    }
}
