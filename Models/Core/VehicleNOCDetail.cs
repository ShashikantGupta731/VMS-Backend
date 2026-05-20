using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    [Table("VehicleNOCDetail")]
    public class VehicleNOCDetail
    {
        [Key]
        public int VehicleNOCDetailId { get; set; }

        public int VehicleInfoId { get; set; }
        
        [ForeignKey("VehicleInfoId")]
        public virtual VehicleInfo VehicleInfo { get; set; } = null!;

        [Column("VehicleNOC")]
        public string VehicleNOC { get; set; } = string.Empty;

        [Column("NOC_IssueDate")]
        public DateTime? NOC_IssueDate { get; set; }

        [Column("NOC_ExpiryDate")]
        public DateTime? NOC_ExpiryDate { get; set; }

        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
