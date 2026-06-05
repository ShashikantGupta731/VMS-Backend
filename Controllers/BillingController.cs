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

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
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

        [HttpGet("fuel/{id}")]
        public async Task<IActionResult> GetFuelBillById(int id)
        {
            var result = await _billingService.GetFuelBillByIdAsync(id, GetUserId());
            if (result == null) return NotFound(new { success = false, msg = "Bill not found" });
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
                var errorMsg = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return BadRequest(new { success = false, msg = errorMsg });
            }
        }

        [HttpDelete("fuel/{id}")]
        public async Task<IActionResult> DeleteFuelBill(int id)
        {
            var result = await _billingService.DeleteFuelBillAsync(id, GetUserId());
            return Ok(new { success = result });
        }

        #endregion

        #region Personal Usage

        [HttpPost("personal-usage")]
        public async Task<IActionResult> InsertPersonalUseDetails(PersonalUsagePayloadDto dto)
        {
            try
            {
                var result = await _billingService.InsertPersonalUseDetailsAsync(dto);
                return Ok(new { success = true, result = new[] { new { msg = "Inserted Successfully" } } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Maintenance Bills

        [HttpGet("maintenance")]
        public async Task<IActionResult> GetMaintenanceBills(int? claimId = null)
        {
            var result = await _billingService.GetMaintenanceBillsAsync(GetUserId(), claimId);
            return Ok(new { success = true, result });
        }
        
        [HttpGet("maintenance/{id}")]
        public async Task<IActionResult> GetMaintenanceBillById(int id)
        {
            var result = await _billingService.GetMaintenanceBillByIdAsync(id, GetUserId());
            if (result == null) return NotFound(new { success = false, msg = "Bill not found" });
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

        #region Miscellaneous Bills

        [HttpGet("miscellaneous")]
        public async Task<IActionResult> GetMiscellaneousBills(int? claimId = null)
        {
            var result = await _billingService.GetMiscellaneousBillsAsync(GetUserId(), claimId);
            return Ok(new { success = true, result });
        }

        [HttpPost("miscellaneous")]
        public async Task<IActionResult> SaveMiscellaneousBill(CreateMiscellaneousBillDto dto)
        {
            try
            {
                var result = await _billingService.SaveMiscellaneousBillAsync(dto, GetUserId());
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, msg = ex.Message });
            }
        }

        [HttpDelete("miscellaneous/{id}")]
        public async Task<IActionResult> DeleteMiscellaneousBill(int id)
        {
            var result = await _billingService.DeleteMiscellaneousBillAsync(id, GetUserId());
            return Ok(new { success = result });
        }

        #endregion

        #region Claims

        [HttpPost("claims")]
        public async Task<IActionResult> CreateClaim([FromBody] CreateClaimDto dto)
        {
            var result = await _billingService.CreateClaimAsync(dto, GetUserId());
            return Ok(new { success = true, result });
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetClaims([FromQuery] BillStatus? status, [FromQuery] bool? forwardedToTreasury)
        {
            var result = await _billingService.GetClaimsAsync(GetUserId(), GetUserRole(), status, forwardedToTreasury);
            return Ok(new { success = true, result });
        }

        [HttpGet("claims/pending-integration")]
        [Authorize(Roles = "DDO")]
        public async Task<IActionResult> GetPendingIntegrationClaims()
        {
            var result = await _billingService.GetPendingIntegrationClaimsAsync(GetUserId(), GetUserRole());
            return Ok(new { success = true, result });
        }

        [HttpGet("claims/{id}")]
        public async Task<IActionResult> GetClaimById(int id)
        {
            var result = await _billingService.GetClaimByIdAsync(id);
            if (result == null) return NotFound(new { success = false, message = "Claim not found" });
            return Ok(new { success = true, result });
        }

        [HttpGet("claims/by-number/{claimNumber}")]
        public async Task<IActionResult> GetClaimByNumber(string claimNumber)
        {
            var result = await _billingService.GetClaimByNumberAsync(claimNumber);
            if (result == null) return NotFound(new { success = false, message = "Claim not found" });
            return Ok(new { success = true, result });
        }

        [HttpGet("bills/search")]
        [Authorize(Roles = "ADMN")]
        public async Task<IActionResult> SearchBillById([FromQuery] int id)
        {
            var result = await _billingService.SearchBillByIdAsync(id);
            return Ok(new { success = true, result });
        }

        [HttpPut("fuel-bills/{id}/odometer")]
        [Authorize(Roles = "ADMN")]
        public async Task<IActionResult> UpdateFuelBillOdometer(int id, [FromBody] UpdateOdometerDto dto)
        {
            var result = await _billingService.UpdateFuelBillOdometerAsync(id, dto);
            if (!result) return NotFound(new { success = false, message = "Fuel bill not found." });
            return Ok(new { success = true });
        }

        [HttpPut("maintenance-bills/{id}/odometer")]
        [Authorize(Roles = "ADMN")]
        public async Task<IActionResult> UpdateMaintenanceBillOdometer(int id, [FromBody] UpdateOdometerDto dto)
        {
            var result = await _billingService.UpdateMaintenanceBillOdometerAsync(id, dto);
            if (!result) return NotFound(new { success = false, message = "Maintenance bill not found." });
            return Ok(new { success = true });
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

        [HttpPost("claims/{id}/discard")]
        [Authorize(Roles = "DDO")]
        public async Task<IActionResult> DiscardClaim(int id, [FromBody] string? comments)
        {
            var result = await _billingService.DiscardClaimAsync(id, GetUserId(), comments);
            return Ok(new { success = result });
        }

        [HttpPost("claims/{id}/restore")]
        [Authorize(Roles = "DDO")]
        public async Task<IActionResult> RestoreClaim(int id, [FromBody] string? comments)
        {
            var result = await _billingService.RestoreClaimAsync(id, GetUserId(), comments);
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
