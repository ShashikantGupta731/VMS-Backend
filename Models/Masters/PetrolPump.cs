using System.ComponentModel.DataAnnotations;

namespace backend.Models.Masters
{
    public class PetrolPump
    {
        [Key]
        public int PetrolPumpId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string GSTIN { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
