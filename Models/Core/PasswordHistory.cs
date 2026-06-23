using System;

namespace backend.Models.Core
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User? User { get; set; }
    }
}
