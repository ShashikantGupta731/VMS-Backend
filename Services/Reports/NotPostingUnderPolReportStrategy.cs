using backend.DTOs.Reports;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class NotPostingUnderPolReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public NotPostingUnderPolReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "NotPostingUnderPOL";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // Find active vehicles that have NO FuelEntry records (not posting fuel/POL data)
            var query = _context.Vehicles
                .Include(v => v.Department)
                .Where(v => v.IsActive && 
                            !_context.FuelEntries.Any(f => f.IsActive && f.VehicleInfoId == v.VehicleInfoId));

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }

            var groupedData = await query
                .GroupBy(v => v.DeptId)
                .Select(g => new
                {
                    DepartmentName = g.FirstOrDefault()!.Department != null ? g.FirstOrDefault()!.Department!.DeptName : "Unknown",
                    NoOfVehicles = g.Count()
                })
                .ToListAsync();

            var data = groupedData.Select(v => new Dictionary<string, object>
            {
                { "DepartmentName", v.DepartmentName },
                { "NoOfVehicles", v.NoOfVehicles }
            }).ToList();

            var headers = new List<string>
            {
                "DepartmentName",
                "NoOfVehicles"
            };

            return new GenericReportResponseDto
            {
                Title = "Vehicles Not Posting Under POL",
                Headers = headers,
                Data = data
            };
        }
    }
}
