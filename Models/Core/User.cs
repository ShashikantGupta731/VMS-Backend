using backend.Models.Masters;

namespace backend.Models.Core
{
    public class User
    {
        public int UserId { get; set; }                              // Renamed from Id for legacy compatibility
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // Full Name
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? EmailId { get; set; }                 // Renamed from Email for legacy compatibility
        public string? PhoneNo { get; set; }                  // Renamed from Phone for legacy compatibility
        
        // Jurisdiction Links
        public int? DistrictId { get; set; }
        public District? District { get; set; }
        
        public int? DeptId { get; set; }                      // Renamed from DepartmentId for legacy compatibility
        public Department? Department { get; set; }
        
        public string DDOCode { get; set; } = string.Empty; // Drawing & Disbursing Officer code
        public string? DDORegistrationNo { get; set; }       // DDO Registration Number (legacy field)
        public string? ManagedDdos { get; set; } // JSON string of managed DDOs for NDOF roles
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;   // Renamed from CreatedAt for legacy compatibility
        
        // Account security
        public int FailedAttempts { get; set; } = 0;
        public DateTime? LockUntil { get; set; } = null;
        public bool Enabled { get; set; } = true;                   // Renamed from IsActive for legacy compatibility
        public bool IsGuest { get; set; } = false;
        public bool IsNonTreasuryDDO { get; set; } = false;
        
        // Navigation property for many-to-many relationship with roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<PasswordHistory> PasswordHistories { get; set; } = new List<PasswordHistory>();
    }
}

