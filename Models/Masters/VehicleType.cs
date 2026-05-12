namespace backend.Models.Masters
{
    public class VehicleType
    {
        // Primary Key - matching legacy VehicletypeId
        public int VehicleTypeId { get; set; }
        
        // Legacy-compatible field names
        public string VehicleTypeName { get; set; } = string.Empty;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
        
        // Additional legacy fields
        public string VehicleTypeExample { get; set; } = string.Empty;
        public int VehicleLifeKM { get; set; } = 0;
        public int VehicleLifeYears { get; set; } = 0;
        
        // Status field - keeping IsActive for now
        public bool IsActive { get; set; } = true;
    }
}
