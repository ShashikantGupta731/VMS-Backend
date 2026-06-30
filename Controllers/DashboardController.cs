using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs.Dashboard;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("GetSummary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            try
            {
                // Extract user claims
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var ddoCode = User.FindFirst("DDOCode")?.Value;
                var deptIdStr = User.FindFirst("DeptId")?.Value;

                if (!int.TryParse(userIdStr, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid user token" });
                }

                int? deptId = null;
                if (int.TryParse(deptIdStr, out int parsedDeptId))
                {
                    deptId = parsedDeptId;
                }

                var summary = await _dashboardService.GetSummaryAsync(userId, role ?? "", ddoCode ?? "", deptId);
                return Ok(new { success = true, result = summary });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching dashboard summary", error = ex.Message });
            }
        }
    }
}
