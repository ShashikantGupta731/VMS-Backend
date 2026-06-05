using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class VehicleTransferReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public VehicleTransferReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "VehicleTransfers";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.VehicleTransfers
                .Include(vt => vt.Vehicle)
                .Include(vt => vt.FromOffice)
                    .ThenInclude(o => o.Department)
                .Include(vt => vt.ToOffice)
                    .ThenInclude(o => o.Department)
                .AsNoTracking()
                .AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue)
            {
                query = query.Where(vt => 
                    (vt.FromOffice != null && vt.FromOffice.DeptId == deptId) || 
                    (vt.ToOffice != null && vt.ToOffice.DeptId == deptId));
            }

            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue)
            {
                query = query.Where(vt => vt.FromOfficeId == officeId || vt.ToOfficeId == officeId);
            }

            var vehicleNo = request.Filters.GetString("vehicleNumber");
            if (!string.IsNullOrEmpty(vehicleNo))
            {
                query = query.Where(vt => vt.Vehicle != null && vt.Vehicle.VehicleNumber.Contains(vehicleNo));
            }

            var transfers = await query.ToListAsync();

            var response = new GenericReportResponseDto
            {
                Title = "Vehicle Transfer History Report",
                Headers = new List<string> { "Vehicle Number", "From Department", "From Office", "To Department", "To Office", "Transfer Date", "Remarks" },
                Data = new List<Dictionary<string, object>>()
            };

            foreach (var t in transfers)
            {
                response.Data.Add(new Dictionary<string, object>
                {
                    { "Vehicle Number", t.Vehicle?.VehicleNumber ?? "N/A" },
                    { "From Department", t.FromOffice?.Department?.DeptName ?? "N/A" },
                    { "From Office", t.FromOffice?.OfficeName ?? "N/A" },
                    { "To Department", t.ToOffice?.Department?.DeptName ?? "N/A" },
                    { "To Office", t.ToOffice?.OfficeName ?? "N/A" },
                    { "Transfer Date", t.TransferDate.ToString("yyyy-MM-dd") },
                    { "Remarks", t.Remarks ?? "N/A" }
                });
            }

            return response;
        }
    }
}
