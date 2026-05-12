using backend.Models.Masters;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class HiredVehicle
    {
        // Primary Key - matching legacy RecordId
        public int HiredVehicleId { get; set; }
        
        // --- 1. Basic Information ---
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        
        // --- 2. Vehicle Details (Foreign Keys) ---
        public int OfficeId { get; set; }
        public Office Office { get; set; } = null!;
        
        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; } = null!;
        
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; } = null!;
        
        public int ModelId { get; set; }
        public VehicleModel Model { get; set; } = null!;
        
        // --- 3. Vehicle Specifications ---
        public int SeatingCapacity { get; set; }
        public string FuelUsed { get; set; } = string.Empty;
        
        // --- 4. Contractor Information ---
        public string ContractorName { get; set; } = string.Empty;
        public string ContractorPhoneNumber { get; set; } = string.Empty;
        
        // --- 5. Billing Period ---
        public DateTime BillDateFrom { get; set; }
        public DateTime BillDateTo { get; set; }
        
        // --- 6. Usage & Cost ---
        public string KMCovered { get; set; } = string.Empty;
        public int BillAmount { get; set; }
        public int Noofvehicles { get; set; }
        
        // --- 7. Legacy Date Fields ---
        public DateTime? PDate { get; set; } // Legacy creation date
        public DateTime? TDate { get; set; } // Legacy termination date
        
        // --- 8. System Fields ---
        public string CreatedBy { get; set; } = string.Empty; // Username of creator
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
