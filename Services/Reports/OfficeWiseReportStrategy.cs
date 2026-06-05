using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class OfficeWiseReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "OfficeWise";

        public OfficeWiseReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Vehicles.Include(v => v.Office).AsNoTracking().AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }

            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(v => v.OfficeId == officeId);
            }

            var groupedData = await query
                .GroupBy(v => v.OfficeId)
                .Select(g => new
                {
                    OfficeId = g.Key,
                    OfficeName = g.FirstOrDefault()!.Office != null ? g.FirstOrDefault()!.Office!.OfficeName : "Unknown",
                    TotalVehicles = g.Count()
                })
                .OrderBy(g => g.OfficeName)
                .ToListAsync();

            var data = groupedData.Select(g => new Dictionary<string, object>
            {
                { "OfficeName", g.OfficeName ?? "Unknown" },
                { "NoOfVehicles", g.TotalVehicles }
            }).ToList();

            var headers = new List<string> { "OfficeName", "NoOfVehicles" };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
