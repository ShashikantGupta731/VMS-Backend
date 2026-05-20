using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class PersonalUsagePlan
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int VehicleInfoId { get; set; }
        
        public string VehicleNo { get; set; } = string.Empty;
        
        public int PlanId { get; set; } // e.g. 1 = Basic, 2 = Premium
        
        public string RecordId { get; set; } = string.Empty; // Links to the draft fuel bill
        
        public string ItemId { get; set; } = string.Empty; // Links to the specific fuel voucher
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<PersonalUsageLog> UsageLogs { get; set; } = new List<PersonalUsageLog>();
    }
}
