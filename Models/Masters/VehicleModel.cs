using System.ComponentModel.DataAnnotations;

namespace backend.Models.Masters
{
    public class VehicleModel
    {
        // Primary Key - matching legacy modelid
        [Key]
        public int ModelId { get; set; }
        
        // Legacy-compatible field names
        public string ModelName { get; set; } = string.Empty;
        
        // Foreign Keys - using legacy naming
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; } = null!;
        
        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; } = null!;
        
        // Legacy field name
        public int SeatingCapacity { get; set; } = 5;
        
        // Status field - keeping IsActive for now
        public bool IsActive { get; set; } = true;
    }
}
