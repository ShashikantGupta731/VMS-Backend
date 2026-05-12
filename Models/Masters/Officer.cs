namespace backend.Models.Masters
{
    public class Officer
    {
        // Primary Key - same as legacy
        public int OfficerId { get; set; }
        
        // Legacy Officer ID (string field from legacy system)
        public string OfficerIdString { get; set; } = string.Empty;
        
        // Single name field (legacy compatibility)
        public string OfficerName { get; set; } = string.Empty;
        
        // HRMS Code (same as legacy)
        public string HrmsCode { get; set; } = string.Empty;
        
        // Direct Department FK (legacy compatibility)
        public int DeptId { get; set; }
        public Department Department { get; set; } = null!;
        
        // Designation FK (same as legacy)
        public int DesignationId { get; set; }
        public Designation Designation { get; set; } = null!;
        
        // Designation Type (legacy field)
        public int DesignationType { get; set; }
        
        // Fuel Limits - changed to int (legacy compatibility)
        public int FuelLimit { get; set; } = 0;              // Petrol Fuel Limit
        public int FuelLimmitd { get; set; } = 0;             // Diesel Fuel Limit (note: typo in original)
        
        // Maintenance Limits - changed to int (legacy compatibility)
        public int MaintenanceLimit { get; set; } = 0;        // Petrol Maintenance Limit
        public int MaintenanceLimitd { get; set; } = 0;       // Diesel Maintenance Limit (note: typo in original)
        
        // Additional legacy fields
        public string Remarks { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        
        // Termination Date (legacy field)
        public DateTime? TDate { get; set; }
        
        // Status field (legacy compatibility)
        public bool Enabled { get; set; } = true;
    }
}
