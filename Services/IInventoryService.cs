using backend.Models.Core;

namespace backend.Services
{
    public interface IInventoryService
    {
        // Item Masters
        Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync();
        Task<InventoryItem> AddInventoryItemAsync(InventoryItem item);
        
        // Stock Management
        Task<IEnumerable<StockTransaction>> GetStockHistoryAsync(int userId);
        Task<int> GetCurrentStockAsync(int userId, int itemId);
        Task<StockTransaction> AddStockAsync(int userId, int itemId, int quantity, string? billNo, DateTime? billDate, string? remarks);
        
        // Allotment
        Task<InventoryAllotment> AllotItemAsync(int userId, int vehicleId, int itemId, int quantity, string? odometer, string? remarks);
        Task<IEnumerable<InventoryAllotment>> GetVehicleAllotmentHistoryAsync(int vehicleId);
        Task<IEnumerable<InventoryAllotment>> GetDdoAllotmentHistoryAsync(int userId);
    }
}
