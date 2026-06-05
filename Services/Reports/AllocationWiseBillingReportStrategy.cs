using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class AllocationWiseBillingReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public AllocationWiseBillingReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "AllocationWiseBilling";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var deptId = request.Filters.GetInt("deptId");
            var officeId = request.Filters.GetInt("officeId");

            var vehiclesQuery = _context.Vehicles.AsNoTracking().AsQueryable();
            if (deptId.HasValue) vehiclesQuery = vehiclesQuery.Where(v => v.DeptId == deptId);
            if (officeId.HasValue) vehiclesQuery = vehiclesQuery.Where(v => v.OfficeId == officeId);

            var vehicles = await vehiclesQuery.ToListAsync();

            var fuelQuery = _context.FuelBills.Include(f => f.Vehicle).AsNoTracking().AsQueryable();
            if (deptId.HasValue) fuelQuery = fuelQuery.Where(f => f.Vehicle.DeptId == deptId);
            if (officeId.HasValue) fuelQuery = fuelQuery.Where(f => f.Vehicle.OfficeId == officeId);
            var fuelBills = await fuelQuery.ToListAsync();

            var maintQuery = _context.MaintenanceBills.Include(m => m.Vehicle).AsNoTracking().AsQueryable();
            if (deptId.HasValue) maintQuery = maintQuery.Where(m => m.Vehicle.DeptId == deptId);
            if (officeId.HasValue) maintQuery = maintQuery.Where(m => m.Vehicle.OfficeId == officeId);
            var maintBills = await maintQuery.ToListAsync();

            var grouped = vehicles
                .GroupBy(v => string.IsNullOrEmpty(v.AllocationType) ? "Unallocated" : v.AllocationType)
                .Select(g => new
                {
                    AllocationType = g.Key,
                    TotalVehicles = g.Count(),
                    FuelBilled = fuelBills.Where(f => f.Vehicle != null && f.Vehicle.AllocationType == g.Key).Sum(f => f.Amount),
                    MaintenanceBilled = maintBills.Where(m => m.Vehicle != null && m.Vehicle.AllocationType == g.Key).Sum(m => m.Amount)
                })
                .OrderBy(g => g.AllocationType)
                .ToList();

            var response = new GenericReportResponseDto
            {
                Title = "Allocation Wise Billing Report",
                Headers = new List<string> { "Allocation Type", "Total Vehicles", "Total Fuel Billed (?)", "Total Maintenance Billed (?)", "Grand Total (?)" },
                Data = new List<Dictionary<string, object>>()
            };

            foreach (var item in grouped)
            {
                response.Data.Add(new Dictionary<string, object>
                {
                    { "Allocation Type", item.AllocationType },
                    { "Total Vehicles", item.TotalVehicles },
                    { "Total Fuel Billed (?)", item.FuelBilled },
                    { "Total Maintenance Billed (?)", item.MaintenanceBilled },
                    { "Grand Total (?)", item.FuelBilled + item.MaintenanceBilled }
                });
            }

            return response;
        }
    }
}
