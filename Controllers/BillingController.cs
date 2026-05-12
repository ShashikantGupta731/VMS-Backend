using System.Security.Claims;
using backend.DTOs.Billing;
using backend.Models.Core;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly IVehicleService _vehicleService;

        public BillingController(IBillingService billingService, IVehicleService vehicleService)
        {
            _billingService = billingService;
            _vehicleService = vehicleService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue("Id") ?? "0");
        private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role) ?? "";
        private string GetUserDdoCode() => User.FindFirstValue("DDOCode") ?? "";

        [HttpGet("vehicles")]
        public async Task<IActionResult> GetDdoVehicles()
        {
            var result = await _vehicleService.GetVehiclesByDdoAsync(GetUserDdoCode());
            return Ok(new { success = true, result });
        }

        #region Fuel Bills

        [HttpGet("fuel")]
        public async Task<IActionResult> GetFuelBills(int? claimId = null)
        {
            var result = await _billingService.GetFuelBillsAsync(GetUserId(), claimId);
            return Ok(new { success = true, result });
        }

        [HttpPost("fuel")]
        public async Task<IActionResult> SaveFuelBill(CreateFuelBillDto dto)
        {
            try
            {
                var result = await _billingService.SaveFuelBillAsync(dto, GetUserId());
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, msg = ex.Message });
            }
        }

        [HttpDelete("fuel/{id}")]
        public async Task<IActionResult> DeleteFuelBill(int id)
        {
            var result = await _billingService.DeleteFuelBillAsync(id, GetUserId());
            return Ok(new { success = result });
        }

        #endregion

        #region Maintenance Bills

        [HttpGet("maintenance")]
        public async Task<IActionResult> GetMaintenanceBills(int? claimId = null)
        {
            var result = await _billingService.GetMaintenanceBillsAsync(GetUserId(), claimId);
            return Ok(new { success = true, result });
        }

        [HttpPost("maintenance")]
        public async Task<IActionResult> SaveMaintenanceBill(CreateMaintenanceBillDto dto)
        {
            try
            {
                var result = await _billingService.SaveMaintenanceBillAsync(dto, GetUserId());
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, msg = ex.Message });
            }
        }

        [HttpDelete("maintenance/{id}")]
        public async Task<IActionResult> DeleteMaintenanceBill(int id)
        {
            var result = await _billingService.DeleteMaintenanceBillAsync(id, GetUserId());
            return Ok(new { success = result });
        }

        #endregion

        #region Hired Vehicle Bills

        [HttpGet("hired")]
        public async Task<IActionResult> GetHiredVehicleBills(int? claimId = null)
        {
            var result = await _billingService.GetHiredVehicleBillsAsync(GetUserId(), claimId);
            return Ok(new { success = true, result });
        }

        [HttpPost("hired")]
        public async Task<IActionResult> SaveHiredVehicleBill(CreateHiredVehicleBillDto dto)
        {
            try
            {
                var result = await _billingService.SaveHiredVehicleBillAsync(dto, GetUserId());
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, msg = ex.Message });
            }
        }

        [HttpDelete("hired/{id}")]
        public async Task<IActionResult> DeleteHiredVehicleBill(int id)
        {
            var result = await _billingService.DeleteHiredVehicleBillAsync(id, GetUserId());
            return Ok(new { success = result });
        }

        #endregion

        #region Contractual Bills

        [HttpGet("contractual")]
        public async Task<IActionResult> GetContractualBills(int? claimId = null)
        {
            var result = await _billingService.GetContractualBillsAsync(GetUserId(), claimId);
            return Ok(new { success = true, result });
        }

        [HttpPost("contractual")]
        public async Task<IActionResult> SaveContractualBill(CreateContractualBillDto dto)
        {
            try
            {
                var result = await _billingService.SaveContractualBillAsync(dto, GetUserId());
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, msg = ex.Message });
            }
        }

        [HttpDelete("contractual/{id}")]
        public async Task<IActionResult> DeleteContractualBill(int id)
        {
            var result = await _billingService.DeleteContractualBillAsync(id, GetUserId());
            return Ok(new { success = result });
        }

        #endregion

        #region Claims

        [HttpPost("claims")]
        public async Task<IActionResult> CreateClaim(CreateClaimDto dto)
        {
            var result = await _billingService.CreateClaimAsync(dto, GetUserId());
            return Ok(new { success = true, result });
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetClaims()
        {
            var result = await _billingService.GetClaimsAsync(GetUserId(), GetUserRole());
            return Ok(new { success = true, result });
        }

        [HttpGet("claims/{id}")]
        public async Task<IActionResult> GetClaimById(int id)
        {
            var result = await _billingService.GetClaimByIdAsync(id);
            if (result == null) return NotFound(new { success = false, msg = "Claim not found" });
            return Ok(new { success = true, result });
        }

        [HttpPost("claims/{id}/verify")]
        [Authorize(Roles = "ADMN,NDOF")]
        public async Task<IActionResult> VerifyClaim(int id, [FromBody] string? comments)
        {
            var result = await _billingService.VerifyClaimAsync(id, GetUserId(), comments);
            return Ok(new { success = result });
        }

        [HttpPost("claims/{id}/reject")]
        [Authorize(Roles = "ADMN,NDOF")]
        public async Task<IActionResult> RejectClaim(int id, [FromBody] string comments)
        {
            var result = await _billingService.RejectClaimAsync(id, GetUserId(), comments);
            return Ok(new { success = result });
        }

        #endregion

        [HttpGet("validate-odometer/{vehicleId}")]
        public async Task<IActionResult> ValidateOdometer(int vehicleId, [FromQuery] DateTime billDate)
        {
            var result = await _billingService.GetOdometerValidationAsync(vehicleId, billDate);
            return Ok(new { success = true, result });
        }

        [HttpGet("check-fitness/{vehicleId}")]
        public async Task<IActionResult> CheckFitness(int vehicleId, [FromQuery] int odometer)
        {
            var result = await _billingService.CheckFitnessRequirementAsync(vehicleId, odometer);
            return Ok(new { success = true, required = result });
        }

        [HttpGet("check-permission/{vehicleId}")]
        public async Task<IActionResult> CheckPermission(int vehicleId, [FromQuery] BillType type, [FromQuery] decimal value)
        {
            var result = await _billingService.CheckPermissionRequirementAsync(vehicleId, type, value);
            return Ok(new { success = true, required = result });
        }
    }
}
