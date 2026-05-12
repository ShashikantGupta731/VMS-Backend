namespace backend.Models.Masters
{
    public class Designation
    {
        // Primary Key - renamed from Id to DesignationId
        public int DesignationId { get; set; }
        
        // Renamed from Name to DesignationName
        public string DesignationName { get; set; } = string.Empty;
        
        // Foreign Key - renamed from DepartmentId to DeptId (matching Department table)
        public int DeptId { get; set; }
        public Department Department { get; set; } = null!;

        public string DesignationType { get; set; } = "Employee";
        
        // NEW: Legacy date fields (exact names from VMS_Backend)
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Fuel Limits - changed from decimal to int (matching VMS_Backend)
        public int PetrolFuelLimit { get; set; } = 0;
        public int DieselFuelLimit { get; set; } = 0;

        // Maintenance Limits - changed from decimal to int (matching VMS_Backend)
        public int PetrolMaintenanceLimit { get; set; } = 0;
        public int DieselMaintenanceLimit { get; set; } = 0;
        
        // Renamed from IsActive to Enabled (matching legacy pattern)
        public bool Enabled { get; set; } = true;
    }
}