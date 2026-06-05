using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class FuelLimitExceededReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public FuelLimitExceededReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "FuelLimitExceeded";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // Determine filter month and year
            int month = DateTime.UtcNow.Month;
            int year = DateTime.UtcNow.Year;

            var filterMonth = request.Filters.GetInt("month");
            if (filterMonth.HasValue) {
                month = filterMonth.Value;
            }
            var filterYear = request.Filters.GetInt("year");
            if (filterYear.HasValue) {
                year = filterYear.Value;
            }

            var startOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

            // Fetch fuel bills grouped by VehicleId for the selected month
            var fuelAggregates = await _context.FuelBills
                .Where(fb => fb.BillDate >= startOfMonth && fb.BillDate <= endOfMonth)
                .GroupBy(fb => fb.VehicleId)
                .Select(g => new
                {
                    VehicleId = g.Key,
                    TotalLitres = g.Sum(fb => fb.FuelQuantity),
                    TotalAmount = g.Sum(fb => fb.Amount)
                })
                .ToListAsync();

            // Fetch vehicles and load relationships
            var vehiclesQuery = _context.Vehicles
                .Include(v => v.Department)
                .Include(v => v.Office)
                .Include(v => v.Officer)
                .Include(v => v.Designation)
                .Include(v => v.Project)
                .AsQueryable();

            // Apply filters
            var deptId = request.Filters.GetInt("deptId"); // also captures departmentId implicitly
            if (deptId.HasValue) {
                vehiclesQuery = vehiclesQuery.Where(v => v.DeptId == deptId);
            }
            var distId = request.Filters.GetInt("districtId");
            if (distId.HasValue) {
                vehiclesQuery = vehiclesQuery.Where(v => v.Office.DistrictId == distId);
            }

            var vehicles = await vehiclesQuery.ToListAsync();

            var response = new GenericReportResponseDto
            {
                Title = $"Vehicles Exceeding Fuel Limits for {startOfMonth:MMMM yyyy}",
                Headers = new List<string> { "Vehicle Number", "Department", "Office", "Officer Name", "Fuel Used", "Assigned Limit (Litres)", "Consumed (Litres)", "Total Cost" },
                Data = new List<Dictionary<string, object>>()
            };

            foreach (var agg in fuelAggregates)
            {
                var v = vehicles.FirstOrDefault(x => x.VehicleInfoId == agg.VehicleId);
                if (v == null) continue;

                // Determine limit based on allocation priority: Officer -> Designation -> Project
                decimal assignedLimit = 0;
                string fuelType = string.IsNullOrEmpty(v.FuelUsed) ? "petrol" : v.FuelUsed.ToLower();

                if (v.Officer != null)
                {
                    assignedLimit = fuelType == "diesel" ? v.Officer.FuelLimmitd : v.Officer.FuelLimit;
                }
                else if (v.Designation != null)
                {
                    assignedLimit = fuelType == "diesel" ? v.Designation.DieselFuelLimit : v.Designation.PetrolFuelLimit;
                }
                else if (v.Project != null)
                {
                    assignedLimit = (decimal)v.Project.FuelLitresPerMonth;
                }

                // If consumed litres exceed the assigned limit, add to report
                if (assignedLimit > 0 && agg.TotalLitres > assignedLimit)
                {
                    response.Data.Add(new Dictionary<string, object>
                    {
                        { "DepartmentName", v.Department?.DeptName ?? "N/A" },
                        { "VehicleNumber", v.VehicleNumber ?? "N/A" },
                        { "AllocatedLimitInLtrs", assignedLimit },
                        { "TotalConsumedInLtrs", Math.Round(agg.TotalLitres, 2) },
                        { "ExceededFuelLimitInLtrs", Math.Round(agg.TotalLitres - assignedLimit, 2) }
                    });
                }
            }

            response.Headers = new List<string> { "DepartmentName", "VehicleNumber", "AllocatedLimitInLtrs", "TotalConsumedInLtrs", "ExceededFuelLimitInLtrs" };
            return response;
        }
    }
}
