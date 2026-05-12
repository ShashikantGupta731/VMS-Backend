using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Masters
{
    public class Project
    {
        // Primary Key - matching legacy ProjectId
        public int ProjectId { get; set; }
        
        // Legacy-compatible field names
        public string ProjectName { get; set; } = string.Empty;
        
        // Legacy fuel and maintenance limits
        public double FuelLitresPerMonth { get; set; } = 0;
        public double MaintenanceAmtPerMonth { get; set; } = 0;
        public double MaintenanceAmtPerAnnum { get; set; } = 0;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Foreign Key to Department
        public int DeptId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
