using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class VehicleExpenditureReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public VehicleExpenditureReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "VehicleExpenditure";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var deptId = request.Filters.GetInt("deptId");
            var officeId = request.Filters.GetInt("officeId");
            var vehicleNo = request.Filters.GetString("vehicleNumber");

            var vehiclesQuery = _context.Vehicles
                .Include(v => v.Department)
                .Include(v => v.Office)
                .AsNoTracking()
                .AsQueryable();

            if (deptId.HasValue) vehiclesQuery = vehiclesQuery.Where(v => v.DeptId == deptId);
            if (officeId.HasValue) vehiclesQuery = vehiclesQuery.Where(v => v.OfficeId == officeId);
            if (!string.IsNullOrEmpty(vehicleNo)) vehiclesQuery = vehiclesQuery.Where(v => v.VehicleNumber.Contains(vehicleNo));

            var vehicles = await vehiclesQuery.ToListAsync();
            var vehicleIds = vehicles.Select(v => v.VehicleInfoId).ToList();

            var fuelBills = await _context.FuelBills
                .Where(f => vehicleIds.Contains(f.VehicleId))
                .GroupBy(f => f.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(f => f.Amount) })
                .ToDictionaryAsync(g => g.VehicleId, g => g.Total);

            var maintBills = await _context.MaintenanceBills
                .Where(m => vehicleIds.Contains(m.VehicleId))
                .GroupBy(m => m.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(m => m.Amount) })
                .ToDictionaryAsync(g => g.VehicleId, g => g.Total);

            var response = new GenericReportResponseDto
            {
                Title = "Overall Vehicle Expenditure Report",
                Headers = new List<string> { "Vehicle Number", "Department", "Office", "Fuel Expense (?)", "Maintenance Expense (?)", "Total Expenditure (?)" },
                Data = new List<Dictionary<string, object>>()
            };

            foreach (var v in vehicles)
            {
                var fuel = fuelBills.ContainsKey(v.VehicleInfoId) ? fuelBills[v.VehicleInfoId] : 0;
                var maint = maintBills.ContainsKey(v.VehicleInfoId) ? maintBills[v.VehicleInfoId] : 0;
                var total = fuel + maint;

                if (total > 0 || !string.IsNullOrEmpty(vehicleNo))
                {
                    response.Data.Add(new Dictionary<string, object>
                    {
                        { "Vehicle Number", v.VehicleNumber ?? "N/A" },
                        { "Department", v.Department?.DeptName ?? "N/A" },
                        { "Office", v.Office?.OfficeName ?? "N/A" },
                        { "Fuel Expense (?)", fuel },
                        { "Maintenance Expense (?)", maint },
                        { "Total Expenditure (?)", total }
                    });
                }
            }

            return response;
        }
    }
}
