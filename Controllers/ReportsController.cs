using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("allocation-wise")]
        public async Task<IActionResult> GetAllocationWise(int? distId, int? deptId)
        {
            var data = await _reportService.GetAllocationTypeWiseCountAsync(distId, deptId);
            return Ok(data);
        }

        [HttpGet("expenditure")]
        public async Task<IActionResult> GetExpenditure(int? distId, int? deptId, DateTime? from, DateTime? to)
        {
            var data = await _reportService.GetVoucherTypeBillsAmountAsync(distId, deptId, from, to);
            return Ok(data);
        }

        [HttpGet("odometer-issues")]
        public async Task<IActionResult> GetOdometerIssues(int? distId, int? deptId)
        {
            var data = await _reportService.GetIncorrectOdometerReadingCountAsync(distId, deptId);
            return Ok(data);
        }

        [HttpGet("unverified")]
        public async Task<IActionResult> GetUnverified(int? distId, int? deptId)
        {
            var data = await _reportService.GetUnverifiedVehiclesCountAsync(distId, deptId);
            return Ok(data);
        }

        [HttpGet("dept-wise")]
        public async Task<IActionResult> GetDeptWise(int? deptId)
        {
            var data = await _reportService.GetVehicleDeptWiseCountAsync(deptId);
            return Ok(data);
        }
    }
}
