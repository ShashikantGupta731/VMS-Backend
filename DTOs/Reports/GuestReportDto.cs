using System;

namespace backend.DTOs.Reports
{
    public class GuestReportRequestDto
    {
        public int VehicleInfoId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class GuestReportResponseDto
    {
        public int VehicleInfoId { get; set; }
        public string VehicleNumber { get; set; }
        public int TotalLitres { get; set; }
        public string TotalAmt { get; set; }
        public int MaxOdometer { get; set; }
    }
}
