using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace backend.DTOs
{
    public class MarkForCondemnedDto
    {
        [Required]
        public string VehicleNumber { get; set; } = string.Empty;
    }

    public class RegisterReplacementVehicleDto
    {
        [Required]
        public string CondemnedVehicleRegNo { get; set; } = string.Empty;
        
        [Required]
        public string NewVehicleRegNo { get; set; } = string.Empty;
        
        public string? NewVehicleChassisNo { get; set; }
        
        public IFormFile? FdApprovalDoc { get; set; }
    }

    public class VehicleGrnDetailsDto
    {
        [Required]
        public string OldVehicleNumber { get; set; } = string.Empty;

        public int? ReplacementVehicleId { get; set; }
        
        [StringLength(50)]
        public string GRNNumber { get; set; } = string.Empty;

        public DateTime? GRNDate { get; set; }
        
        public decimal? GRNBillAmount { get; set; }

        public bool HaveYouEnteredAllGRNsFullAmount { get; set; }
        
        public decimal AmountForSelectedVehicle { get; set; }

        public bool IsOldGrn { get; set; }
        
        public IFormFile? GrnDoc { get; set; }
    }
}
