using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    [Table("GuestAccessLogs")]
    public class GuestAccessLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int VehicleInfoId { get; set; }

        [MaxLength(100)]
        public string GuestName { get; set; }

        [MaxLength(20)]
        public string GuestMobileNo { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }

        public DateTime DateFrom { get; set; }
        
        public DateTime DateTo { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
