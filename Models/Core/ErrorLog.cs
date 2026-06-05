using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    [Table("ErrorLogs")]
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ProjectModuleId { get; set; }
        
        [MaxLength(255)]
        public string? ErrRoute { get; set; }
        
        public string? ErrDesc { get; set; }
        
        public string? ErrException { get; set; }
        
        [MaxLength(50)]
        public string? ErrIp { get; set; }

        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(100)]
        public string? UserId { get; set; }

        public string? RequestParameter { get; set; }

        public DateTime? ErrDate { get; set; } = DateTime.UtcNow;
    }
}
