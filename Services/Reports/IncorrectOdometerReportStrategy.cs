using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class IncorrectOdometerReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public IncorrectOdometerReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "IncorrectOdometer";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // Apply optional filters
            var query = _context.Vehicles
                .Include(v => v.Office)
                .Include(v => v.Department)
                .AsQueryable();

            var deptId = request.Filters.GetInt("deptId"); // checks both deptId and departmentId implicitly
            if (deptId.HasValue)
            {
                query = query.Where(v => v.DeptId == deptId);
            }

            // Example business logic for incorrect odometer (Reading is missing or anomaly)
            // Legacy app might have specific logic; here we demonstrate the architecture.
            query = query.Where(v => string.IsNullOrEmpty(v.FinancialYearReading) || v.FinancialYearReading == "0");

            var vehicles = await query.ToListAsync();

            var response = new GenericReportResponseDto
            {
                Title = "Vehicles with Incorrect or Missing Odometer Readings",
                Headers = new List<string> { "VehicleNumber", "RegistrationDate", "DepartmentName", "CurrentMeterReading", "MeterReadingDate", "FuelFilledAmount", "BillAmount" },
                Data = new List<Dictionary<string, object>>()
            };

            foreach (var v in vehicles)
            {
                response.Data.Add(new Dictionary<string, object>
                {
                    { "VehicleNumber", v.VehicleNumber ?? "N/A" },
                    { "RegistrationDate", v.VehiclePurchaseDate?.ToString("yyyy-MM-dd") ?? "N/A" },
                    { "DepartmentName", v.Department?.DeptName ?? "N/A" },
                    { "CurrentMeterReading", v.ReadingUptodate?.ToString("yyyy-MM-dd") ?? "Missing" },
                    { "MeterReadingDate", "N/A" },
                    { "FuelFilledAmount", 0 },
                    { "BillAmount", 0 }
                });
            }

            return response;
        }
    }
}
