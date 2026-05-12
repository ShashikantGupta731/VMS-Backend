namespace backend.DTOs.User
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        
        public int? DistrictId { get; set; }
        public int? DepartmentId { get; set; }
        public string DDOCode { get; set; } = string.Empty;
        public string? DDORegistrationNo { get; set; }
        public string? ManagedDdos { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool IsNonTreasuryDDO { get; set; } = false;
        
        public List<string> Roles { get; set; } = new List<string>();
    }
}
