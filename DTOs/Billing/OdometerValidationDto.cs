using backend.Models.Core;

namespace backend.DTOs.Billing
{
    public class OdometerValidationDto
    {
        public int LastReading { get; set; }
        public DateTime? LastReadingDate { get; set; }
        public bool IsValid { get; set; }
    }
}
