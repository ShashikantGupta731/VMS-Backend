using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class FitnessCertificateReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "FitnessCertificate";

        public FitnessCertificateReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.VehicleNOCDetails
                .Include(noc => noc.VehicleInfo)
                    .ThenInclude(v => v.Department)
                .Include(noc => noc.VehicleInfo)
                    .ThenInclude(v => v.Office)
                        .ThenInclude(o => o.District)
                .AsNoTracking()
                .AsQueryable();

            // Filters
            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(noc => noc.VehicleInfo.DeptId == deptId);
            }
            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(noc => noc.VehicleInfo.OfficeId == officeId);
            }
            var vehicleNo = request.Filters.GetString("vehicleNumber");
            if (!string.IsNullOrEmpty(vehicleNo)) {
            
                query = query.Where(noc => noc.VehicleInfo.VehicleNumber.Contains(vehicleNo));
            }

            var nocData = await query.ToListAsync();

            var data = nocData.Select(noc => new Dictionary<string, object>
            {
                { "VehicleNumber", noc.VehicleInfo?.VehicleNumber ?? "" },
                { "Department", noc.VehicleInfo?.Department?.DeptName ?? "" },
                { "Office", noc.VehicleInfo?.Office?.OfficeName ?? "" },
                { "District", noc.VehicleInfo?.Office?.District?.DistrictName ?? "" },
                { "VehicleNOC", noc.VehicleNOC },
                { "NOCIssueDate", noc.NOC_IssueDate?.ToString("yyyy-MM-dd") ?? "" },
                { "NOCExpiryDate", noc.NOC_ExpiryDate?.ToString("yyyy-MM-dd") ?? "" },
                { "IsExpired", (noc.NOC_ExpiryDate.HasValue && noc.NOC_ExpiryDate.Value < System.DateTime.UtcNow) ? "Yes" : "No" }
            }).ToList();

            var headers = new List<string>
            {
                "VehicleNumber", "Department", "Office", "District", "VehicleNOC",
                "NOCIssueDate", "NOCExpiryDate", "IsExpired"
            };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
