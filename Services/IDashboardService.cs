using backend.DTOs.Dashboard;

namespace backend.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int userId, string role, string ddoCode, int? deptId);
    }
}
