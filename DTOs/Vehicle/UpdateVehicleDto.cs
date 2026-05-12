using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace backend.DTOs
{
    public class UpdateVehicleDto
    {
        // Purchase Info
        [Required]
        public bool Ishaveyoupurchasednewvehicle { get; set; }
        public string? FdApproval { get; set; }
        public string? VehiclePurchaseType { get; set; }
        public string? OtherPurchaseTypeDetails { get; set; }
        public int? NewFleetStrength { get; set; }
        public string? FleetStrengthLetter { get; set; }
        public string? CondemnedVehicleRegNo { get; set; }
        public string? CondemnedVehicleChassisNo { get; set; }
        public string? VehicleSource { get; set; }
        
        // Office & Assignment
        [Required]
        public int OfficeId { get; set; }
        public string? CurrentStatus { get; set; }
        public string? VehicleAllocationType { get; set; }
        public int? DesignationId { get; set; }
        public string? OfficerName { get; set; }
        public int? OfficerId { get; set; }
        public string? Project { get; set; }
        public int? ProjectId { get; set; }
        public string? HRMSCode { get; set; }
        public string? DriverType { get; set; }
        public string? DriverName { get; set; }
        public string? DriverContactNo { get; set; }
        public string? ContractorName { get; set; }
        public string? ContractorContactNo { get; set; }
        public string? Department { get; set; }
        public string? VehicleOwnerOffice { get; set; }
        
        // Registration Details
        public string? RegistrationType { get; set; }
        [Required]
        public string VehicleNumber { get; set; } = string.Empty;
        public string? ManufactureYear { get; set; }
        public int? SeatingCapacity { get; set; }
        
        // Vehicle Details
        [Required]
        public int VehicleTypeId { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        public int? ModelId { get; set; }
        
        // Uploads (IFormFile for direct uploads)
        public IFormFile? VehiclePhoto { get; set; }
        public IFormFile? RegistrationCertificate { get; set; }
        public IFormFile? FdApprovalFile { get; set; }
        public IFormFile? FleetStrengthLetterFile { get; set; }
        
        // Technical Details
        public string? ChassisNumber { get; set; }
        public decimal? VehicleCost { get; set; }
        [Required]
        public string FuelUsed { get; set; } = string.Empty;
        
        // Dates & Usage
        public DateTime? PurchaseDate { get; set; }
        public DateTime? FitnessUpto { get; set; }
        public int? KM30062017 { get; set; }
        
        // Fuel & Maintenance
        public decimal? FuelConsumptionCost { get; set; }
        public decimal? FuelConsumptionLitres { get; set; }
        public decimal? Last3YearsMaintenanceCost { get; set; }
        
        // Tyre Details
        public bool IsTyreOriginal { get; set; }
        public DateTime? TyreChangedDate { get; set; }
        public int? TyreChangedMeterReading { get; set; }
    }
}
