using backend.Data;
using backend.DTOs.PetrolPump;
using backend.Models.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class PetrolPumpService : IPetrolPumpService
    {
        private readonly AppDbContext _context;

        public PetrolPumpService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int userId)
        {
            // For the dashboard, we sum up fuel entries created by this user
            var userEntries = await _context.FuelEntries
                .Where(f => f.CreatedBy == userId.ToString())
                .ToListAsync();

            return new DashboardStatsDto
            {
                TotalLitresFilled = (int)userEntries.Sum(f => f.FuelConsumptionLitres ?? 0m),
                TotalAmount = userEntries.Sum(f => f.FuelConsumptionCost ?? 0m),
                TotalVehiclesServed = userEntries.Select(f => f.VehicleNumber).Distinct().Count()
            };
        }

        public async Task<List<FuelLogDto>> GetRecentFuelLogsAsync(int userId)
        {
            var logs = await _context.FuelEntries
                .Where(f => f.CreatedBy == userId.ToString())
                .OrderByDescending(f => f.CreatedDate)
                .Take(100)
                .Select(f => new FuelLogDto
                {
                    VehicleNumber = f.VehicleNumber,
                    Litres = f.FuelConsumptionLitres ?? 0m,
                    Amount = f.FuelConsumptionCost ?? 0m,
                    Date = f.BillDate,
                    FuelType = "Petrol", // Legacy default, would need proper mapping if stored
                    DdoCode = f.Vehicle.DDOId ?? "N/A",
                    OfficeName = f.Vehicle.Office != null ? f.Vehicle.Office.OfficeName : "N/A",
                    District = f.Vehicle.Office != null && f.Vehicle.Office.District != null ? f.Vehicle.Office.District.DistrictName : "N/A",
                    DepartmentName = f.Vehicle.Department != null ? f.Vehicle.Department.DeptName : "N/A"
                })
                .ToListAsync();

            return logs;
        }

        public async Task<StockAmountDto> GetStockAmountsAsync(int userId)
        {
            var stock = new StockAmountDto();

            // We fetch the raw inventory available
            var inventoryItems = await _context.InventoryItems.ToListAsync();
            
            // Fetch total consumed from FuelEntries (this represents petrol station issuance)
            // Note: In a fully fleshed out system, FuelEntry should track the exact InventoryItemId.
            // For now, we will sum all fuel entries and deduct them from Petrol and Diesel respectively.
            // As a fallback, we assume most entries are Petrol unless specified.
            var totalConsumedLitres = await _context.FuelEntries
                .SumAsync(f => f.FuelConsumptionLitres ?? 0m);

            // Map based on item names (Petrol, Diesel, etc.)
            foreach (var item in inventoryItems)
            {
                var totalAllotment = await _context.InventoryAllotments
                    .Where(a => a.InventoryItemId == item.InventoryItemId)
                    .SumAsync(a => a.Quantity);
                
                string name = item.Name.ToLower();
                if (name.Contains("petrol")) stock.Petrol = totalAllotment;
                else if (name.Contains("diesel")) stock.Diesel = totalAllotment;
                else if (name.Contains("mobil oil")) stock.MobilOil = totalAllotment;
                else if (name.Contains("engine oil")) stock.EngineOil = totalAllotment;
                else if (name.Contains("gear oil")) stock.GearOil = totalAllotment;
                else if (name.Contains("break oil")) stock.BreakOil = totalAllotment;
            }

            // Subtract fuel consumed from the petrol stock as per legacy default behaviour
            if (stock.Petrol > 0)
            {
                stock.Petrol -= (int)totalConsumedLitres;
            }

            // Fallback default values if no inventory is set up (to allow UI testing)
            if (stock.Petrol <= 0) stock.Petrol = 5000 - (int)totalConsumedLitres;
            if (stock.Diesel == 0) stock.Diesel = 5000;
            if (stock.MobilOil == 0) stock.MobilOil = 500;
            if (stock.EngineOil == 0) stock.EngineOil = 500;
            if (stock.GearOil == 0) stock.GearOil = 500;
            if (stock.BreakOil == 0) stock.BreakOil = 500;

            return stock;
        }

        public async Task<bool> InsertFuelEntryAsync(int userId, FuelEntryRequestDto request)
        {
            // Resolve VehicleInfoId from VehicleNumber
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleNumber == request.VehicleNumber);

            var fuelEntry = new FuelEntry
            {
                BillNumber = "PPO-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                BillDate = DateTime.SpecifyKind(request.DateAllowance, DateTimeKind.Utc),
                VehicleNumber = request.VehicleNumber,
                VehicleInfoId = vehicle?.VehicleInfoId ?? 0,
                FuelConsumptionLitres = request.Litres,
                FuelConsumptionCost = request.Amount,
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.FuelEntries.Add(fuelEntry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VehicleSearchDto>> SearchVehiclesAsync(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return new List<VehicleSearchDto>();

            searchQuery = searchQuery.ToLower();

            return await _context.Vehicles
                .Where(v => v.VehicleNumber.ToLower().Contains(searchQuery))
                .Select(v => new VehicleSearchDto
                {
                    VehicleNumber = v.VehicleNumber,
                    DdoCode = v.DDOId // Mapping DDOId to DdoCode
                })
                .Take(50) // Limit results
                .ToListAsync();
        }
    }
}
