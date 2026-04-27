namespace backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Account security
        public int FailedAttempts { get; set; } = 0;
        public DateTime? LockUntil { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public bool IsGuest { get; set; } = false;
        
        // Navigation property for many-to-many relationship with roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}