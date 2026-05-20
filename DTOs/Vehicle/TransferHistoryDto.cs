using System;

namespace backend.DTOs.Vehicle
{
    public class TransferHistoryDto
    {
        public string VehicleTransferredOn { get; set; } = string.Empty;
        public string VerifiedOn { get; set; } = string.Empty;
        public string PreviousOffice { get; set; } = string.Empty;
        public string PreviousDepartment { get; set; } = string.Empty;
        public string AllocationType { get; set; } = string.Empty;
        public string OfficerName { get; set; } = string.Empty;
        public string OfficerDesignation { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string PreviousDdo { get; set; } = string.Empty;
        public string NewDdo { get; set; } = string.Empty;
    }
}
