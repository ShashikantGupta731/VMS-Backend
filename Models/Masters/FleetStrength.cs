using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Masters
{
    public class FleetStrength
    {
        // Primary Key - matching legacy FleetStrengthId
        public int FleetStrengthId { get; set; }
        
        // Foreign Keys - matching legacy field names
        public int DeptId { get; set; }
        public int DistrictId { get; set; }
        public int VehicleTypeId { get; set; }
        
        // Legacy data field
        public int FleetStrengthValue { get; set; }
        
        // Legacy date fields for consistency with other models
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Navigation properties
        [ForeignKey("DeptId")]
        public Department? Department { get; set; }
        
        [ForeignKey("DistrictId")]
        public District? District { get; set; }
        
        [ForeignKey("VehicleTypeId")]
        public VehicleType? VehicleType { get; set; }
    }
}
