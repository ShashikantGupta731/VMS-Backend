using System;

namespace backend.DTOs.Vehicle
{
    public class BillRecordDto
    {
        public string RecordId { get; set; } = string.Empty;
        public string ClaimNumber { get; set; } = string.Empty;
        public string SubVoucherNo { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int OdometerReading { get; set; }
        public string SanctionOrderNo { get; set; } = string.Empty;
        public string SanctionOrderDate { get; set; } = string.Empty;
        public string SanctionAuthority { get; set; } = string.Empty;
        public string PermissionReceived { get; set; } = string.Empty;
        public string PermissionNoc { get; set; } = string.Empty;
        public string VmsEntryDate { get; set; } = string.Empty;
        public decimal FuelConsumptionLitres { get; set; }
    }
}
