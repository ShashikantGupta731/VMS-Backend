using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class VehicleFuelInfoReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "VehicleFuelInfo";

        public VehicleFuelInfoReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.FuelBills
                .Include(fb => fb.Vehicle)
                    .ThenInclude(v => v.Department)
                .Include(fb => fb.Vehicle)
                    .ThenInclude(v => v.Office)
                        .ThenInclude(o => o.District)
                .AsNoTracking()
                .AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(fb => fb.Vehicle.DeptId == deptId);
            }
            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(fb => fb.Vehicle.OfficeId == officeId);
            }
            var vehicleNo = request.Filters.GetString("vehicleNumber");
            if (!string.IsNullOrEmpty(vehicleNo)) {
            
                query = query.Where(fb => fb.Vehicle.VehicleNumber.Contains(vehicleNo));
            }

            var bills = await query.ToListAsync();

            var data = bills.Select(fb => new Dictionary<string, object>
            {
                { "VehicleNumber", fb.Vehicle?.VehicleNumber ?? "" },
                { "Department", fb.Vehicle?.Department?.DeptName ?? "" },
                { "Office", fb.Vehicle?.Office?.OfficeName ?? "" },
                { "District", fb.Vehicle?.Office?.District?.DistrictName ?? "" },
                { "BillNumber", fb.BillNumber },
                { "BillDate", fb.BillDate.ToString("yyyy-MM-dd") },
                { "FuelQuantity", fb.FuelQuantity },
                { "Amount", fb.Amount },
                { "OdometerReading", fb.OdometerReading },
                { "Status", fb.Status.ToString() }
            }).ToList();

            var headers = new List<string>
            {
                "VehicleNumber", "Department", "Office", "District",
                "BillNumber", "BillDate", "FuelQuantity", "Amount", "OdometerReading", "Status"
            };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
