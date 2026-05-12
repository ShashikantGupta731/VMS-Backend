using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace backend.DTOs
{
    public class CondemnVehicleDto
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public DateTime CondemnationDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CondemnationOrderNumber { get; set; } = string.Empty;

        public IFormFile? CondemnationOrderFile { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string? AuctionStatus { get; set; }
        public DateTime? AuctionDate { get; set; }
        public decimal? AuctionAmount { get; set; }
    }
}
