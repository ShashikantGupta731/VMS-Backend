using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class DistrictWiseReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "DistrictWise";

        public DistrictWiseReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Vehicles.Include(v => v.Office).ThenInclude(o => o.District).AsNoTracking().AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }

            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(v => v.OfficeId == officeId);
            }

            var groupedData = await query
                .GroupBy(v => v.Office != null ? v.Office.DistrictId : null)
                .Select(g => new
                {
                    DistrictId = g.Key,
                    DistrictName = g.FirstOrDefault()!.Office != null && g.FirstOrDefault()!.Office!.District != null ? g.FirstOrDefault()!.Office!.District!.DistrictName : "Unknown",
                    TotalVehicles = g.Count()
                })
                .OrderBy(g => g.DistrictName)
                .ToListAsync();

            var data = groupedData.Select(g => new Dictionary<string, object>
            {
                { "DistrictName", g.DistrictName ?? "Unknown" },
                { "NoOfVehicles", g.TotalVehicles }
            }).ToList();

            var headers = new List<string> { "DistrictName", "NoOfVehicles" };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
