namespace backend.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public VehicleSummaryDto VehicleSummary { get; set; } = new VehicleSummaryDto();
        public BillingSummaryDto BillingSummary { get; set; } = new BillingSummaryDto();
        public UserSummaryDto UserSummary { get; set; } = new UserSummaryDto();
        public List<ActivityDto> RecentActivity { get; set; } = new List<ActivityDto>();
        public List<PendingActionDto> PendingActions { get; set; } = new List<PendingActionDto>();
    }

    public class VehicleSummaryDto
    {
        public int TotalVehicles { get; set; }
        public int VerifiedVehicles { get; set; }
        public int UnverifiedVehicles { get; set; }
        public int CondemnedVehicles { get; set; }
        public int ActiveVehicles { get; set; }
    }

    public class BillingSummaryDto
    {
        public int PendingBills { get; set; }
        public int BillsReadyForIfms { get; set; }
        public int TotalClaimsThisMonth { get; set; }
        public int PendingClaimVerifications { get; set; }
    }

    public class UserSummaryDto
    {
        public int TotalUsers { get; set; }
        public int ActiveToday { get; set; }
        public int ErrorCountToday { get; set; }
    }

    public class ActivityDto
    {
        public string Action { get; set; }
        public string Detail { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PendingActionDto
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public string Route { get; set; }
    }
}
