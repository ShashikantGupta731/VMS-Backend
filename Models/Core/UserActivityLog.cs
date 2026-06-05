using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    [Table("UserActivityLogs")]
    public class UserActivityLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ProjectModuleId { get; set; }
        
        [MaxLength(100)]
        public string? UserId { get; set; }
        
        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(100)]
        public string? ProfileId { get; set; }
        
        [MaxLength(100)]
        public string? ProfileName { get; set; }

        [MaxLength(50)]
        public string? LoginStatus { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(20)]
        public string? Method { get; set; }

        [MaxLength(255)]
        public string? Api { get; set; }

        [MaxLength(255)]
        public string? Route { get; set; }

        public string? RequestBody { get; set; }

        [MaxLength(50)]
        public string? Latitude { get; set; }

        [MaxLength(50)]
        public string? Longitude { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
