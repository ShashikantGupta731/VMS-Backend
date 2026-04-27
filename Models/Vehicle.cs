namespace backend.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        
        // Purchase Info
        public string PurchasedNewVehicle { get; set; } = string.Empty;
        
        // Office & Assignment
        public string OfficeName { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public string VehicleAllocationType { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string OfficerName { get; set; } = string.Empty;
        public string HrmsCode { get; set; } = string.Empty;
        public string DriverType { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string DriverContactNumber { get; set; } = string.Empty;
        public string ContractorName { get; set; } = string.Empty;
        public string ContractorContactNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string VehicleOwnerOffice { get; set; } = string.Empty;
        
        // Registration Details
        public string RegistrationType { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string ManufactureYear { get; set; } = string.Empty;
        public int? SeatingCapacity { get; set; }
        
        // Vehicle Details
        public string VehicleType { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        
        // Uploads (file paths/URLs)
        public string VehiclePhoto { get; set; } = string.Empty;
        public string RegistrationCertificate { get; set; } = string.Empty;
        
        // Technical Details
        public string ChassisNumber { get; set; } = string.Empty;
        public decimal? VehicleCost { get; set; }
        public string FuelUsed { get; set; } = string.Empty;
        
        // Dates & Usage
        public DateTime? PurchaseDate { get; set; }
        public DateTime? FitnessUpto { get; set; }
        public int? KmsCovered { get; set; }
        
        // Fuel & Maintenance
        public decimal? FuelCostLast3Months { get; set; }
        public decimal? FuelLitresLast3Months { get; set; }
        public decimal? MaintenanceCostLast3Months { get; set; }
        
        // Tyre Details
        public string IsTyreOriginal { get; set; } = string.Empty;
        public DateTime? TyreChangedDate { get; set; }
        public int? TyreChangedMeterReading { get; set; }
        
        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Foreign key to User (who created the vehicle record)
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
    }
}
