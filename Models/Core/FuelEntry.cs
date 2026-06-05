using backend.Models.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class FuelEntry
    {
        // Primary Key - matching legacy RecordId
        public int FuelEntryId { get; set; }
        
        // --- 1. Basic Information ---
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        
        // --- 2. Vehicle Information ---
        public int VehicleInfoId { get; set; }
        public VehicleInfo Vehicle { get; set; } = null!;
        public string VehicleNumber { get; set; } = string.Empty;
        
        // --- 3. Fuel Consumption Details ---
        public int OdometerReading { get; set; }
        
        public decimal? FuelConsumptionLitres { get; set; } // Updated from object?
        
        public decimal? FuelConsumptionCost { get; set; } // Updated from object?
        
        // --- 4. Permission & NOC Details ---
        public string Permission { get; set; } = string.Empty;
        public string Nocfile { get; set; } = string.Empty;
        public DateTime? NocIssueDate { get; set; }
        public DateTime? NocExpiryDate { get; set; }
        public string SanctionAuthorityMobileNo { get; set; } = string.Empty;
        
        // --- 5. Personal Use Tracking ---
        public bool? IsPersonalUsed { get; set; } // Legacy boolean field
        
        // --- 6. Legacy Date Fields ---
        public DateTime? PDate { get; set; } // Legacy creation date
        public DateTime? TDate { get; set; } // Legacy termination date
        
        // --- 7. System Fields ---
        public string CreatedBy { get; set; } = string.Empty; // Username of creator
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
