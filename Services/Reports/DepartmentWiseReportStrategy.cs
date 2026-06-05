using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class DepartmentWiseReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "DepartmentWise";

        public DepartmentWiseReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Vehicles.Include(v => v.Department).AsNoTracking().AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }

            var groupedData = await query
                .GroupBy(v => v.DeptId)
                .Select(g => new
                {
                    DeptId = g.Key,
                    DepartmentName = g.FirstOrDefault()!.Department != null ? g.FirstOrDefault()!.Department!.DeptName : "Unknown",
                    TotalVehicles = g.Count(),
                    Verified = g.Count(v => v.IsVerified == true),
                    UnVerified = g.Count(v => v.IsVerified != true),
                    InUse = g.Count(v => v.CurrentStatus == "Active" || v.CurrentStatus == "In Use" || v.CurrentStatus == "InUse"),
                    NotInUse = g.Count(v => v.CurrentStatus == "Not In Use" || v.CurrentStatus == "NotInUse"),
                    Condemned = g.Count(v => v.CurrentStatus == "Condemned"),
                    MarkedForCondemned = g.Count(v => v.CurrentStatus == "MarkedForCondemned"),
                    Sold = g.Count(v => v.CurrentStatus == "Sold")
                })
                .OrderBy(g => g.DepartmentName)
                .ToListAsync();

            var data = groupedData.Select(g => new Dictionary<string, object>
            {
                { "DepartmentName", g.DepartmentName ?? "Unknown" },
                { "TotalVehicles", g.TotalVehicles },
                { "Verified", g.Verified },
                { "UnVerified", g.UnVerified },
                { "InUse", g.InUse },
                { "NotInUse", g.NotInUse },
                { "Condemned", g.Condemned },
                { "MarkedForCondemnedByFD", g.MarkedForCondemned },
                { "Sold", g.Sold }
            }).ToList();

            var headers = new List<string>
            {
                "DepartmentName", "TotalVehicles", "Verified", "UnVerified", "InUse", "NotInUse", "Condemned", "MarkedForCondemnedByFD", "Sold"
            };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
