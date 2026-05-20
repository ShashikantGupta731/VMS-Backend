using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    public class PersonalUsageLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PersonalUsagePlanId { get; set; }
        
        [Required]
        public string OfficerId { get; set; } = string.Empty; // Ties to the Officer who used it
        
        [Required]
        public DateTime DateOfUse { get; set; }
        
        public decimal OdometerFrom { get; set; }
        
        public decimal OdometerTo { get; set; }
        
        [ForeignKey("PersonalUsagePlanId")]
        public virtual PersonalUsagePlan PersonalUsagePlan { get; set; } = null!;
    }
}
