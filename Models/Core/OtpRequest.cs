using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    [Table("OtpRequests")]
    public class OtpRequest
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string PhoneNo { get; set; } = string.Empty;

        // Note: The user requested to keep OTP in plain text for easy future activation/debugging,
        // but to keep the hashed logic commented out.
        public string OtpCode { get; set; } = string.Empty;

        // public string OtpHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
