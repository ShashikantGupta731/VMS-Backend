using backend.Models.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class FuelMaintenance
    {
        // Primary Key - matching legacy RecordId
        public int FuelMaintenanceId { get; set; }
        
        // --- 1. Basic Information ---
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        
        // --- 2. Vehicle Information ---
        public int VehicleInfoId { get; set; }
        public VehicleInfo Vehicle { get; set; } = null!;
        public string VehicleNumber { get; set; } = string.Empty;
        public int OdometerReading { get; set; }
        
        // --- 3. Maintenance Details ---
        public string Action { get; set; } = string.Empty; // Fuel/Maintenance type
        public int Amount { get; set; }
        public string Details { get; set; } = string.Empty;
        public string MaintenanceType { get; set; } = string.Empty;
        
        // --- 4. Voucher Information ---
        public string SubVoucherNo { get; set; } = string.Empty;
        public string SubVoucherDesc { get; set; } = string.Empty;
        public string ExpenditureDetails { get; set; } = string.Empty;
        
        // --- 5. Sanction Information ---
        public string SanctionOrderNo { get; set; } = string.Empty;
        public DateTime? SanctionOrderDate { get; set; }
        public string SanctionAuthority { get; set; } = string.Empty;
        public string FirmName { get; set; } = string.Empty;
        
        // --- 6. Financial Processing ---
        public bool? FwdToTreasury { get; set; }
        public string ClaimInfo { get; set; } = string.Empty;
        public bool? IsProduction { get; set; }
        public bool? IsGrantInAidBill { get; set; }
        
        [NotMapped]
        public object? AmountSanctioned { get; set; } // Legacy dynamic type
        
        public string GrantInAidPeriod { get; set; } = string.Empty;
        public string SanctionedBy { get; set; } = string.Empty;
        public string FDSanctionLetterNo { get; set; } = string.Empty;
        public DateTime? DateOfIssue { get; set; }
        public string Purpose { get; set; } = string.Empty;
        
        [NotMapped]
        public object? Deductions { get; set; } // Legacy dynamic type
        
        // --- 7. Bill Processing ---
        public bool? IsBulkBill { get; set; }
        public bool? IsSupplementaryBill { get; set; }
        public string ParentClaimId { get; set; } = string.Empty;
        public bool? SupplementaryAllotmentDone { get; set; }
        
        // --- 8. IFMS Integration ---
        public int IFMSStatus { get; set; }
        public long? IFMSBillNo { get; set; }
        public bool? IsNewIFMS { get; set; }
        public string BillInfoDetail { get; set; } = string.Empty;
        public long? VMSRefNo { get; set; }
        public DateTime? BillSubmittedDate { get; set; }
        
        // --- 9. Tax Information ---
        public int IncomeTaxAmount { get; set; }
        
        // --- 10. Claim Information ---
        public string ClaimNo { get; set; } = string.Empty;
        public string ClaimResponseId { get; set; } = string.Empty;
        public int ClaimVerificationStatus { get; set; }
        
        // --- 11. Legacy Date Fields ---
        public DateTime? PDate { get; set; } // Legacy creation date
        public DateTime? TDate { get; set; } // Legacy termination date
        
        // --- 12. System Fields ---
        public string CreatedBy { get; set; } = string.Empty; // Username of creator
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
