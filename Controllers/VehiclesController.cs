using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using backend.DTOs;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VehicleResponseDto>>> GetAllVehicles([FromQuery] int? status)
        {
            return Ok(await _vehicleService.GetAllVehiclesAsync(status));
        }

        [HttpGet("by-ddo/{ddoCode}")]
        public async Task<ActionResult<List<VehicleResponseDto>>> GetVehiclesByDdo(string ddoCode)
        {
            return Ok(await _vehicleService.GetVehiclesByDdoAsync(ddoCode));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleResponseDto>> GetVehicleById(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        [HttpPost]
        [Authorize(Roles = "DDO")]
        public async Task<ActionResult<VehicleResponseDto>> CreateVehicle([FromForm] CreateVehicleDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var vehicle = await _vehicleService.CreateVehicleAsync(dto, userId);
            return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.Id }, vehicle);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "DDO")]
        public async Task<ActionResult<VehicleResponseDto>> UpdateVehicle(int id, [FromForm] UpdateVehicleDto dto)
        {
            var vehicle = await _vehicleService.UpdateVehicleAsync(id, dto);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMN")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/verify")]
        [Authorize(Roles = "ADMN,DDO")]
        public async Task<IActionResult> VerifyVehicle(int id)
        {
            var verifierId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var result = await _vehicleService.VerifyVehicleAsync(id, verifierId, "Verified by Admin", 1);
            if (!result) return NotFound();
            return Ok(new { message = "Vehicle verified successfully" });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "ADMN,DDO")]
        public async Task<IActionResult> RejectVehicle(int id, [FromBody] RejectVehicleDto dto)
        {
            var verifierId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var result = await _vehicleService.VerifyVehicleAsync(id, verifierId, dto.Comments, 2);
            if (!result) return NotFound();
            return Ok(new { message = "Vehicle rejected with comments" });
        }

        [HttpPost("transfer")]
        [Authorize(Roles = "DDO")]
        public async Task<IActionResult> TransferVehicle([FromForm] TransferVehicleDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _vehicleService.TransferVehicleAsync(dto, userId);
            return result ? Ok(new { message = "Vehicle transferred successfully" }) : BadRequest(new { message = "Transfer failed" });
        }

        [HttpPost("register-replacement")]
        [Authorize(Roles = "DDO,ADMN")]
        public async Task<IActionResult> RegisterReplacementVehicle([FromForm] RegisterReplacementVehicleDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var result = await _vehicleService.RegisterReplacementVehicleAsync(dto, userId);
            return result ? Ok(new { success = true, message = "Replacement vehicle registered successfully" }) : BadRequest(new { success = false, message = "Failed to register replacement vehicle" });
        }

        [HttpPost("mark-for-condemned")]
        [Authorize(Roles = "DDO,ADMN,FD")]
        public async Task<IActionResult> MarkForCondemned([FromBody] MarkForCondemnedDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var result = await _vehicleService.MarkForCondemnedAsync(dto, userId);
            return result ? Ok(new { success = true, message = "Vehicle marked for condemned successfully" }) : BadRequest(new { success = false, message = "Failed to mark vehicle" });
        }

        [HttpPost("reject-condemnation")]
        [Authorize(Roles = "FD")]
        public async Task<IActionResult> RejectCondemnation([FromBody] RejectCondemnationDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var success = await _vehicleService.RejectCondemnationAsync(dto.VehicleNumber, dto.Reason, userId);
            if (!success) return BadRequest(new { message = "Failed to reject condemnation request." });
            return Ok(new { message = "Condemnation request rejected successfully." });
        }

        [HttpPost("add-grn-details")]
        [Authorize(Roles = "DDO,ADMN")]
        public async Task<IActionResult> AddVehicleGrnNumberAndDetails([FromForm] VehicleGrnDetailsDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var result = await _vehicleService.AddVehicleGrnNumberAndDetailsAsync(dto, userId);
            return result ? Ok(new { success = true, message = "GRN details added successfully" }) : BadRequest(new { success = false, message = "Failed to add GRN details" });
        }

        [HttpGet("{id}/fitness-certificates")]
        public async Task<IActionResult> GetFitnessCertificates(int id)
        {
            var certificates = await _vehicleService.GetFitnessCertificatesAsync(id);
            return Ok(certificates);
        }

        [HttpGet("{id}/fuel-bills")]
        public async Task<IActionResult> GetFuelBills(int id)
        {
            var bills = await _vehicleService.GetFuelBillsAsync(id);
            return Ok(bills);
        }

        [HttpGet("{id}/maintenance-bills")]
        public async Task<IActionResult> GetMaintenanceBills(int id)
        {
            var bills = await _vehicleService.GetMaintenanceBillsAsync(id);
            return Ok(bills);
        }

        [HttpGet("{id}/service-bills")]
        public async Task<IActionResult> GetServiceBills(int id)
        {
            var bills = await _vehicleService.GetServiceBillsAsync(id);
            return Ok(bills);
        }

        [HttpGet("{id}/battery-changes")]
        public async Task<IActionResult> GetBatteryChanges(int id)
        {
            var bills = await _vehicleService.GetBatteryChangesAsync(id);
            return Ok(bills);
        }

        [HttpGet("{id}/tyre-changes")]
        public async Task<IActionResult> GetTyreChanges(int id)
        {
            var bills = await _vehicleService.GetTyreChangesAsync(id);
            return Ok(bills);
        }

        [HttpGet("{id}/transfer-history")]
        public async Task<IActionResult> GetTransferHistory(int id)
        {
            var history = await _vehicleService.GetTransferHistoryAsync(id);
            return Ok(history);
        }
    }
}
