using System;

namespace backend.DTOs.Vehicle
{
    public class FitnessCertificateDto
    {
        public int VehicleNOCDetailId { get; set; }
        public string CertificateIssuedDate { get; set; } = string.Empty;
        public string CertificateExpiryDate { get; set; } = string.Empty;
        public string Certificate { get; set; } = string.Empty;
    }
}
