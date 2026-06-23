using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Masters
{
    public class Secretary
    {
        [Key]
        public int SecretaryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string EmailId { get; set; } = string.Empty;

        public int DeptId { get; set; }

        [ForeignKey("DeptId")]
        public virtual Department Department { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
