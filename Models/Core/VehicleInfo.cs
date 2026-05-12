using backend.Models.Masters;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class VehicleInfo
    {
        // Primary Key - matching legacy VehicleInfoId
        public int VehicleInfoId { get; set; }
        
        // --- 1. Vehicle Identity (Linked to Masters) ---
        public string VehicleNumber { get; set; } = string.Empty;
        public string EngineChasisNumber { get; set; } = string.Empty;
        
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; } = null!;
        
        public int ModelId { get; set; }
        public VehicleModel Model { get; set; } = null!;
        
        public int ManufactureYear { get; set; }
        
        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; } = null!;
        
        public string SeatingCapacity { get; set; } = string.Empty;
        public string FuelUsed { get; set; } = string.Empty; // Petrol/Diesel/EV
        
        // --- 2. Allocation & Assignment ---
        public int OfficeId { get; set; }
        public Office Office { get; set; } = null!;
        
        // Legacy-compatible foreign key
        public int? DeptId { get; set; }
        public Department? Department { get; set; }
        
        public int? OfficerId { get; set; }
        public Officer? Officer { get; set; }
        
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
        
        public int? DesignationId { get; set; }
        public Designation? Designation { get; set; }
        
        public string OfficerName { get; set; } = string.Empty;
        public string HRMSCode { get; set; } = string.Empty;
        public string AllocationType { get; set; } = string.Empty; // Earmarked/Pooled
        
        // --- 3. Status & Verification ---
        public string CurrentStatus { get; set; } = "Active"; // Active/Condemned
        
        // Legacy-compatible verification fields
        public int verificationstatus { get; set; } = 0; // 0=Unverified, 1=Verified, 2=Rejected
        public bool? IsVerified { get; set; } = false; // Legacy verification flag
        public string VerifierId { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty; // Legacy field name
        public DateTime? VerificationDate { get; set; }
        
        // --- 4. Driver Details (Denormalized for quick access) ---
        public string DriverType { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string DriverContactNo { get; set; } = string.Empty;
        public string ContractorName { get; set; } = string.Empty;
        public string ContractorContactNo { get; set; } = string.Empty;
        
        // --- 5. Financial & Purchase ---
        [NotMapped]
        public object? VehicleCost { get; set; } // Legacy dynamic type
        public DateTime? VehiclePurchaseDate { get; set; } // Legacy field name
        
        // --- 6. Vehicle Maintenance & Fitness ---
        public DateTime? FitnessUpto { get; set; } // Vehicle fitness expiry
        public bool? Ishaveyoupurchasednewvehicle { get; set; } // Whether purchased as new vehicle
        public bool? IsTyreOriginal { get; set; }
        public DateTime? LastTyreChangedDate { get; set; }
        public int? LastTyreChangedKM { get; set; }
        public int? KM_30062017 { get; set; } // Historical KM reading
        
        // --- 7. Fuel & Consumption Tracking ---
        [NotMapped]
        public object? FuelConsumptionCost { get; set; }
        [NotMapped]
        public object? FuelConsumptionLitres { get; set; }
        public DateTime FuelConsumptionCostDate { get; set; }
        [NotMapped]
        public object? LastThreeYearsMaintenanceCost { get; set; }
        public DateTime LastThreeYearsMaintenanceCostDate { get; set; }
        
        // --- 8. Documents & Proofs ---
        public bool? IsTemporaryRegistration { get; set; }
        public string VehiclePhotoPath { get; set; } = string.Empty;
        public string RegistrationCertificatePath { get; set; } = string.Empty;
        public string FdApprovalPath { get; set; } = string.Empty;
        public string FleetStrengthLetterPath { get; set; } = string.Empty;
        public string vehicleproofuploads { get; set; } = string.Empty; // Legacy field name
        
        // --- 9. Requisition & Tracking ---
        public int? RequisitionDeptId { get; set; }
        public int? RequisitionOfficeId { get; set; }
        public string MaintenenceDuration { get; set; } = string.Empty;
        public DateTime? ReadingUptodate { get; set; }
        public string FinancialYearReading { get; set; } = string.Empty;
        public bool? temporpermanent { get; set; }
        
        // --- 10. NOC & Nodal Officer ---
        public string VehicleNOC { get; set; } = string.Empty;
        public DateTime? NOC_IssueDate { get; set; }
        public string NodalOfficerName { get; set; } = string.Empty;
        public string NodalOfficerMobileNo { get; set; } = string.Empty;
        public string NodalOfficerUsername { get; set; } = string.Empty;
        public string NodalOfficerEmail { get; set; } = string.Empty;
        
        // --- 11. Security & Audit ---
        public string DDOId { get; set; } = string.Empty; // Legacy field name
        public string TreasuryType { get; set; } = string.Empty;
        
        // --- 12. Legacy Date Fields ---
        public DateTime? PDate { get; set; } // Legacy creation date
        public DateTime? TDate { get; set; } // Legacy termination date
        
        // --- 13. System Fields ---
        public string CreatedBy { get; set; } = string.Empty; // Username of creator
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
