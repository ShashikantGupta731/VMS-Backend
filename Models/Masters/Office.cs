namespace backend.Models.Masters
{
    public class Office
    {
        // Primary Key - matching legacy OfficeId
        public int OfficeId { get; set; }
        
        // Legacy-compatible field names
        public string OfficeName { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        
        // Foreign Keys - using legacy naming (Nullable to allow LEFT JOINs with missing data)
        public int? DeptId { get; set; }
        public Department? Department { get; set; }
        
        public int? DistrictId { get; set; }
        public District? District { get; set; }

        public int? TehsilId { get; set; }
        public Tehsil? Tehsil { get; set; }
        
        // Additional legacy fields
        public string OfficeAbbreviation { get; set; } = string.Empty;
        public int OfficeTypeId { get; set; }
        public string OfficeTypeOther { get; set; } = string.Empty;

        // The legacy system tracks which user "owns" or created this office record
        public string UserId { get; set; } = string.Empty;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Status field - using 'Enabled' to match legacy CSV and Departments table
        public bool Enabled { get; set; } = true;
    }
}
