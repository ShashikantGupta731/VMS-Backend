using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class DesignationWiseReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "DesignationWise";

        public DesignationWiseReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Vehicles.Include(v => v.Designation).AsNoTracking().AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }

            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(v => v.OfficeId == officeId);
            }

            var groupedData = await query
                .GroupBy(v => v.DesignationId)
                .Select(g => new
                {
                    DesignationId = g.Key,
                    DesignationName = g.FirstOrDefault()!.Designation != null ? g.FirstOrDefault()!.Designation!.DesignationName : "Unknown",
                    TotalVehicles = g.Count()
                })
                .OrderBy(g => g.DesignationName)
                .ToListAsync();

            var data = groupedData.Select(g => new Dictionary<string, object>
            {
                { "DesignationName", g.DesignationName ?? "Unknown" },
                { "NoOfVehicles", g.TotalVehicles }
            }).ToList();

            var headers = new List<string> { "DesignationName", "NoOfVehicles" };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
