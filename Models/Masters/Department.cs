namespace backend.Models.Masters
{
    public class Department
    {
        // Primary Key - matching legacy DeptId
        public int DeptId { get; set; }
        
        // Legacy-compatible field names
        public string DeptName { get; set; } = string.Empty;        // Legacy field name
        
        // Legacy abbreviation field
        public string DeptAbbre { get; set; } = string.Empty;       // Legacy field name
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Status field - matching legacy Enabled
        public bool? Enabled { get; set; } = true;                   // Nullable to match legacy
    }
}
