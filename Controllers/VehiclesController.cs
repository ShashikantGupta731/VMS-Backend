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
        [Authorize(Roles = "ADMN")]
        public async Task<IActionResult> VerifyVehicle(int id)
        {
            var verifierId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var result = await _vehicleService.VerifyVehicleAsync(id, verifierId, "Verified by Admin", 1);
            if (!result) return NotFound();
            return Ok(new { message = "Vehicle verified successfully" });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "ADMN")]
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

        [HttpPost("condemn")]
        [Authorize(Roles = "DDO")]
        public async Task<IActionResult> CondemnVehicle([FromForm] CondemnVehicleDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _vehicleService.CondemnVehicleAsync(dto, userId);
            return result ? Ok(new { message = "Vehicle condemned successfully" }) : BadRequest(new { message = "Condemn failed" });
        }
    }
}
