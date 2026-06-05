using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class AllocationTypeWiseDataReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "AllocationTypeWiseData";

        public AllocationTypeWiseDataReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Vehicles.AsNoTracking().AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }
            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(v => v.OfficeId == officeId);
            }
            var alloc = request.Filters.GetString("allocationType");
            if (!string.IsNullOrEmpty(alloc)) {
                string allocType = alloc;
                query = query.Where(v => v.AllocationType == allocType);
            }

            var groupedData = await query
                .GroupBy(v => v.AllocationType)
                .Select(g => new
                {
                    AllocationType = g.Key ?? "Unknown",
                    InUse = g.Count(v => v.CurrentStatus == "Active" || v.CurrentStatus == "In Use" || v.CurrentStatus == "InUse"),
                    NotInUse = g.Count(v => v.CurrentStatus == "Not In Use" || v.CurrentStatus == "NotInUse"),
                    Condemned = g.Count(v => v.CurrentStatus == "Condemned"),
                    NoOfVehicles = g.Count()
                })
                .OrderBy(g => g.AllocationType)
                .ToListAsync();

            var data = groupedData.Select(g => new Dictionary<string, object>
            {
                { "AllocationType", g.AllocationType },
                { "InUse", g.InUse },
                { "NotInUse", g.NotInUse },
                { "Condemned", g.Condemned },
                { "NoOfVehicles", g.NoOfVehicles }
            }).ToList();

            var headers = new List<string>
            {
                "AllocationType", "InUse", "NotInUse", "Condemned", "NoOfVehicles"
            };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
