namespace backend.DTOs
{
    public class UpdateDriverDetailsDto
    {
        public string DriverName { get; set; } = string.Empty;
        public string DriverContact { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string DriverType { get; set; } = string.Empty;
        public bool RegistrationType { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Attachments { get; set; } = string.Empty;
    }
}
