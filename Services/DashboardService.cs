using backend.Data;
using backend.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;
using backend.Models.Core;
using backend.Models.Masters;

namespace backend.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int userId, string role, string ddoCode, int? deptId)
        {
            var summary = new DashboardSummaryDto();

            // 1. Vehicle Summary
            IQueryable<VehicleInfo> vehicleQuery = _context.Vehicles;

            // Apply role-based filtering for vehicles
            if (role == "DDO")
            {
                // DDO sees only their own DDO Code or Dept
                var userDept = await _context.Users.Where(u => u.UserId == userId).Select(u => u.DeptId).FirstOrDefaultAsync();
                vehicleQuery = vehicleQuery.Where(v => v.DeptId == userDept);
            }
            else if (role == "SEC" || role == "HOD" || role == "DCL")
            {
                // Dept level
                vehicleQuery = vehicleQuery.Where(v => v.DeptId == deptId);
            }
            // ADMN sees all

            summary.VehicleSummary.TotalVehicles = await vehicleQuery.CountAsync();
            summary.VehicleSummary.VerifiedVehicles = await vehicleQuery.CountAsync(v => v.verificationstatus == 1 || v.IsVerified == true);
            summary.VehicleSummary.UnverifiedVehicles = await vehicleQuery.CountAsync(v => v.verificationstatus == null || v.verificationstatus == 0 || v.IsVerified == false);
            
            // Condemned vehicles typically have a specific status ID or property. For now checking VehicleCondemnations
            // Since Condemned is status based, let's just count from VehicleCondemnations for simplicity
            var condemnedQuery = _context.VehicleCondemnations.AsQueryable();
            if (role == "DDO")
            {
                 var userDept = await _context.Users.Where(u => u.UserId == userId).Select(u => u.DeptId).FirstOrDefaultAsync();
                 condemnedQuery = condemnedQuery.Where(c => c.Vehicle.DeptId == userDept);
            }
            else if (role == "SEC" || role == "HOD" || role == "DCL")
            {
                 condemnedQuery = condemnedQuery.Where(c => c.Vehicle.DeptId == deptId);
            }

            summary.VehicleSummary.CondemnedVehicles = await condemnedQuery.CountAsync();
            summary.VehicleSummary.ActiveVehicles = summary.VehicleSummary.TotalVehicles - summary.VehicleSummary.CondemnedVehicles;


            // 2. Billing Summary
            IQueryable<BillClaim> claimQuery = _context.BillClaims;
            if (role == "DDO")
            {
                claimQuery = claimQuery.Where(c => c.CreatedById == userId);
            }

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            summary.BillingSummary.TotalClaimsThisMonth = await claimQuery.CountAsync(c => c.CreatedAt >= startOfMonth);
            
            // Pending Verification
            summary.BillingSummary.PendingClaimVerifications = await claimQuery.CountAsync(c => c.Status == BillStatus.Pending);
            
            summary.BillingSummary.PendingBills = await claimQuery.CountAsync(c => !c.ForwardedToTreasury);
            summary.BillingSummary.BillsReadyForIfms = await claimQuery.CountAsync(c => c.Status == BillStatus.Verified && !c.ForwardedToTreasury);


            // 3. User Summary (mostly for Admin)
            var userIdStr = userId.ToString();
            if (role == "ADMN")
            {
                summary.UserSummary.TotalUsers = await _context.Users.CountAsync();
                
                var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
                summary.UserSummary.ActiveToday = await _context.UserActivityLogs
                    .Where(l => l.CreatedOn >= today)
                    .Select(l => l.UserId)
                    .Distinct()
                    .CountAsync();

                summary.UserSummary.ErrorCountToday = await _context.ErrorLogs
                    .Where(e => e.ErrDate >= today)
                    .CountAsync();
            }

            // 4. Pending Actions (Actionable alerts)
            if (role == "ADMN")
            {
                if (summary.VehicleSummary.UnverifiedVehicles > 0)
                {
                    summary.PendingActions.Add(new PendingActionDto { Type = "Unverified Vehicles", Count = summary.VehicleSummary.UnverifiedVehicles, Route = "/verify-vehicles" });
                }
            }
            if (role == "NDOF")
            {
                if (summary.BillingSummary.PendingClaimVerifications > 0)
                {
                    summary.PendingActions.Add(new PendingActionDto { Type = "Pending Claims", Count = summary.BillingSummary.PendingClaimVerifications, Route = "/claim-verification" });
                }
            }
            if (role == "DDO")
            {
                if (summary.BillingSummary.BillsReadyForIfms > 0)
                {
                    summary.PendingActions.Add(new PendingActionDto { Type = "Ready for IFMS", Count = summary.BillingSummary.BillsReadyForIfms, Route = "/bill-integration" });
                }
            }


            // 5. Recent Activity
            // Only show relevant mutating actions (exclude GET, OPTIONS, and read-like POSTs)
            var recentLogs = await _context.UserActivityLogs
                .Where(l => role == "ADMN" || l.UserId == userIdStr)
                .Where(l => l.Method != "GET" && l.Method != "OPTIONS")
                .Where(l => l.Route != null && !l.Route.Contains("Get") && !l.Route.Contains("login") && !l.Route.Contains("public-key"))
                .OrderByDescending(l => l.CreatedOn)
                .Take(5)
                .ToListAsync();

            summary.RecentActivity = recentLogs.Select(l => new ActivityDto
            {
                Action = l.Method ?? "Action",
                Detail = l.Route ?? "N/A",
                Timestamp = l.CreatedOn ?? DateTime.UtcNow
            }).ToList();

            return summary;
        }
    }
}
