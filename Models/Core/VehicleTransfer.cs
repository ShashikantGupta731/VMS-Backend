using backend.Models.Masters;

namespace backend.Models.Core
{
    public class VehicleTransfer
    {
        public int VehicleTransferId { get; set; }
        
        public int VehicleId { get; set; }
        public VehicleInfo Vehicle { get; set; } = null!;
        
        public int FromOfficeId { get; set; }
        public Office FromOffice { get; set; } = null!;
        
        public int ToOfficeId { get; set; }
        public Office ToOffice { get; set; } = null!;
        
        public DateTime TransferDate { get; set; }
        public string TransferOrderNumber { get; set; } = string.Empty;
        public string TransferOrderPath { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        
        // Historical Snapshots
        public string FromAllocationType { get; set; } = string.Empty;
        public string FromOfficerName { get; set; } = string.Empty;
        public string FromDesignationName { get; set; } = string.Empty;
        public string FromDdoCode { get; set; } = string.Empty;
        public string ToDdoCode { get; set; } = string.Empty;
        public DateTime? VerificationDate { get; set; }
        
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
