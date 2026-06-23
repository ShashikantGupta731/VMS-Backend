namespace backend.DTOs.Masters
{
    public class SecretaryDto
    {
        public int SecretaryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public int DeptId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateSecretaryDto
    {
        public string Title { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public int DeptId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSecretaryDto
    {
        public string Title { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public int DeptId { get; set; }
        public bool IsActive { get; set; }
    }
}
