using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.VMSMobileApp
{
    public class InDriverLatLongDto
    {
        public long TripId { get; set; }
        
        [Required]
        public int VehicleInfoId { get; set; }
        
        public double Longitude { get; set; }
        
        public double Latitude { get; set; }
        
        public string? DriverName { get; set; }
        
        public string? DriverPhone { get; set; }
        
        public string? OdometerReading { get; set; }
        
        /// <summary>
        /// Supports legacy base64 or string path submission
        /// </summary>
        public string? OdometerImg { get; set; }

        /// <summary>
        /// Modern file upload support
        /// </summary>
        public IFormFile? OdometerImgFile { get; set; }
        
        public bool TripCompleted { get; set; }
    }
    
    public class OutDriverLatLongDto
    {
        public long TripId { get; set; }
        public string Msg { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class InGetVehicleEntryByDriverContactDto
    {
        [Required]
        public string DriverContactNo { get; set; } = string.Empty;
    }

    public class OutGetVehicleEntryByDriverContactDto
    {
        public int? VehicleInfoId { get; set; }
        public string? ManufacturerName { get; set; }
        public string? ModelName { get; set; }
        public string? VehicleNumber { get; set; }
        public string? FuelUsed { get; set; }
        public string? DriverName { get; set; }
        public string? DriverMobileNo { get; set; }
        public string? VehicleProofUploads { get; set; }
    }
}
