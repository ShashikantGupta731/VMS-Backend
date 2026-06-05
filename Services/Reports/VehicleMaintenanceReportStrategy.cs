using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class VehicleMaintenanceReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "VehicleMaintenance";

        public VehicleMaintenanceReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.MaintenanceBills
                .Include(mb => mb.Vehicle)
                    .ThenInclude(v => v.Department)
                .Include(mb => mb.Vehicle)
                    .ThenInclude(v => v.Office)
                        .ThenInclude(o => o.District)
                .AsNoTracking()
                .AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(mb => mb.Vehicle.DeptId == deptId);
            }
            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(mb => mb.Vehicle.OfficeId == officeId);
            }
            var vehicleNo = request.Filters.GetString("vehicleNumber");
            if (!string.IsNullOrEmpty(vehicleNo)) {
            
                query = query.Where(mb => mb.Vehicle.VehicleNumber.Contains(vehicleNo));
            }

            var bills = await query.ToListAsync();

            var data = bills.Select(mb => new Dictionary<string, object>
            {
                { "VehicleNumber", mb.Vehicle?.VehicleNumber ?? "" },
                { "Department", mb.Vehicle?.Department?.DeptName ?? "" },
                { "Office", mb.Vehicle?.Office?.OfficeName ?? "" },
                { "District", mb.Vehicle?.Office?.District?.DistrictName ?? "" },
                { "BillNumber", mb.BillNumber },
                { "BillDate", mb.BillDate.ToString("yyyy-MM-dd") },
                { "MaintenanceType", mb.MaintenanceType },
                { "Amount", mb.Amount },
                { "OdometerReading", mb.OdometerReading },
                { "Status", mb.Status.ToString() }
            }).ToList();

            var headers = new List<string>
            {
                "VehicleNumber", "Department", "Office", "District",
                "BillNumber", "BillDate", "MaintenanceType", "Amount", "OdometerReading", "Status"
            };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
