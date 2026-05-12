using backend.Data;
using backend.Models.Core;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync()
        {
            return await _context.InventoryItems.Where(i => i.IsActive).ToListAsync();
        }

        public async Task<InventoryItem> AddInventoryItemAsync(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<StockTransaction>> GetStockHistoryAsync(int userId)
        {
            return await _context.StockTransactions
                .Include(s => s.InventoryItem)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetCurrentStockAsync(int userId, int itemId)
        {
            // Total Added - Total Allotted
            var added = await _context.StockTransactions
                .Where(s => s.UserId == userId && s.InventoryItemId == itemId)
                .SumAsync(s => s.Quantity);
                
            var allotted = await _context.InventoryAllotments
                .Where(a => a.AllottedByUserId == userId && a.InventoryItemId == itemId)
                .SumAsync(a => a.Quantity);
                
            return added - allotted;
        }

        public async Task<StockTransaction> AddStockAsync(int userId, int itemId, int quantity, string? billNo, DateTime? billDate, string? remarks)
        {
            var transaction = new StockTransaction
            {
                UserId = userId,
                InventoryItemId = itemId,
                Quantity = quantity,
                BillNumber = billNo,
                BillDate = billDate,
                Remarks = remarks
            };

            _context.StockTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<InventoryAllotment> AllotItemAsync(int userId, int vehicleId, int itemId, int quantity, string? odometer, string? remarks)
        {
            // 1. Check stock availability
            int currentStock = await GetCurrentStockAsync(userId, itemId);
            if (currentStock < quantity)
            {
                throw new InvalidOperationException("Insufficient stock for allotment.");
            }

            // 2. Create Allotment
            var allotment = new InventoryAllotment
            {
                AllottedByUserId = userId,
                VehicleId = vehicleId,
                InventoryItemId = itemId,
                Quantity = quantity,
                OdometerReading = odometer,
                Remarks = remarks
            };

            _context.InventoryAllotments.Add(allotment);
            await _context.SaveChangesAsync();
            return allotment;
        }

        public async Task<IEnumerable<InventoryAllotment>> GetVehicleAllotmentHistoryAsync(int vehicleId)
        {
            return await _context.InventoryAllotments
                .Include(a => a.InventoryItem)
                .Include(a => a.AllottedByUser)
                .Where(a => a.VehicleId == vehicleId)
                .OrderByDescending(a => a.AllotmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryAllotment>> GetDdoAllotmentHistoryAsync(int userId)
        {
            return await _context.InventoryAllotments
                .Include(a => a.InventoryItem)
                .Include(a => a.Vehicle)
                .Where(a => a.AllottedByUserId == userId)
                .OrderByDescending(a => a.AllotmentDate)
                .ToListAsync();
        }
    }
}
