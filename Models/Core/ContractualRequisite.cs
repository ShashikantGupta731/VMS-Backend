using backend.Models.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class ContractualRequisite
    {
        // Primary Key - matching legacy RecordId
        public int ContractualRequisiteId { get; set; }
        
        // --- 1. Basic Information ---
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; }
        public DateTime BillDateFrom { get; set; }
        public DateTime BillDateTo { get; set; }
        
        // --- 2. DDO Information ---
        public string DDOCode { get; set; } = string.Empty; // Legacy field name
        
        // --- 3. Vehicle Information ---
        public int VehicleInfoId { get; set; }
        public VehicleInfo Vehicle { get; set; } = null!;
        public string VehicleNumber { get; set; } = string.Empty;
        
        // --- 4. Financial Information ---
        public int BillAmount { get; set; }
        public bool IsContractual { get; set; } // Legacy boolean field
        
        // --- 5. Legacy Date Fields ---
        public DateTime? PDate { get; set; } // Legacy creation date
        public DateTime? TDate { get; set; } // Legacy termination date
        
        // --- 6. System Fields ---
        public string CreatedBy { get; set; } = string.Empty; // Username of creator
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
