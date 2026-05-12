using backend.Models.Core;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetItems()
        {
            var items = await _inventoryService.GetInventoryItemsAsync();
            return Ok(items);
        }

        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] AddStockRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var transaction = await _inventoryService.AddStockAsync(
                userId, request.ItemId, request.Quantity, request.BillNo, request.BillDate, request.Remarks);
            return Ok(transaction);
        }

        [HttpGet("stock-status/{itemId}")]
        public async Task<IActionResult> GetStockStatus(int itemId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var stock = await _inventoryService.GetCurrentStockAsync(userId, itemId);
            return Ok(new { ItemId = itemId, AvailableStock = stock });
        }

        [HttpPost("allot")]
        public async Task<IActionResult> AllotItem([FromBody] AllotItemRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var allotment = await _inventoryService.AllotItemAsync(
                    userId, request.VehicleId, request.ItemId, request.Quantity, request.Odometer, request.Remarks);
                return Ok(allotment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/allotments")]
        public async Task<IActionResult> GetMyAllotments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var history = await _inventoryService.GetDdoAllotmentHistoryAsync(userId);
            return Ok(history);
        }

        [HttpGet("history/vehicle/{vehicleId}")]
        public async Task<IActionResult> GetVehicleHistory(int vehicleId)
        {
            var history = await _inventoryService.GetVehicleAllotmentHistoryAsync(vehicleId);
            return Ok(history);
        }
    }

    public class AddStockRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string? BillNo { get; set; }
        public DateTime? BillDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class AllotItemRequest
    {
        public int VehicleId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string? Odometer { get; set; }
        public string? Remarks { get; set; }
    }
}
