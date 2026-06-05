using backend.DTOs.PetrolPump;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "PPOF,ADMN")] // Allow Petrol Pump Officer and Admin
    public class PetrolPumpController : ControllerBase
    {
        private readonly IPetrolPumpService _petrolPumpService;

        public PetrolPumpController(IPetrolPumpService petrolPumpService)
        {
            _petrolPumpService = petrolPumpService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("Id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out int userId);
            return userId;
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _petrolPumpService.GetDashboardStatsAsync(GetUserId());
            return Ok(new { success = true, result = stats });
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetFuelLogs()
        {
            var logs = await _petrolPumpService.GetRecentFuelLogsAsync(GetUserId());
            return Ok(new { success = true, result = logs });
        }

        [HttpGet("stock")]
        public async Task<IActionResult> GetStockAmounts()
        {
            var stock = await _petrolPumpService.GetStockAmountsAsync(GetUserId());
            return Ok(new { success = true, result = stock });
        }

        [HttpPost("entry")]
        public async Task<IActionResult> InsertFuelEntry([FromBody] FuelEntryRequestDto request)
        {
            var success = await _petrolPumpService.InsertFuelEntryAsync(GetUserId(), request);
            if (success)
            {
                return Ok(new { success = true, msg = "Data Added Successfully" });
            }
            return BadRequest(new { success = false, msg = "Unable to process fuel entry" });
        }

        [HttpGet("search-vehicles")]
        public async Task<IActionResult> SearchVehicles([FromQuery] string query)
        {
            var vehicles = await _petrolPumpService.SearchVehiclesAsync(query);
            return Ok(new { success = true, result = vehicles });
        }
    }
}
