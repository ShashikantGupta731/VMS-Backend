using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class VehicleDetailsReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "VehicleDetails";

        public VehicleDetailsReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Vehicles
                .Include(v => v.Manufacturer)
                .Include(v => v.Model)
                .Include(v => v.VehicleType)
                .Include(v => v.Office)
                    .ThenInclude(o => o.District)
                .Include(v => v.Department)
                .Include(v => v.Officer)
                .Include(v => v.Designation)
                .AsNoTracking()
                .AsQueryable();

            // Apply Filters
            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }

            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(v => v.OfficeId == officeId);
            }

            var vehicleNo = request.Filters.GetString("vehicleNumber");
            if (!string.IsNullOrEmpty(vehicleNo)) {
                query = query.Where(v => v.VehicleNumber.Contains(vehicleNo));
            }

            var searchType = request.Filters.GetString("searchType");
            var searchValue = request.Filters.GetString("searchValue");
            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchValue)) {
                if (searchType.Equals("vehicle", StringComparison.OrdinalIgnoreCase) || searchType.Equals("Vehicle Wise", StringComparison.OrdinalIgnoreCase)) {
                    query = query.Where(v => v.VehicleNumber.Contains(searchValue));
                }
                else if (searchType.Equals("officer", StringComparison.OrdinalIgnoreCase) || searchType.Equals("Officer Wise", StringComparison.OrdinalIgnoreCase)) {
                    query = query.Where(v => (!string.IsNullOrEmpty(v.OfficerName) && v.OfficerName.Contains(searchValue)) || 
                                          (v.Officer != null && v.Officer.OfficerName.Contains(searchValue)));
                }
                else if (searchType.Equals("designation", StringComparison.OrdinalIgnoreCase) || searchType.Equals("Designation Wise", StringComparison.OrdinalIgnoreCase)) {
                    query = query.Where(v => v.Designation != null && v.Designation.DesignationName.Contains(searchValue));
                }
            }

            var vehicles = await query.ToListAsync();

            var data = vehicles.Select(v => new Dictionary<string, object>
            {
                { "VehicleNoModelNoMfYearChassisNo", $"{v.VehicleNumber ?? "-"} / {v.Model?.ModelName ?? "-"} / {v.ManufactureYear} / {v.EngineChasisNumber ?? "-"}" },
                { "DepartmentNameOfficeDDOCode", $"{v.Department?.DeptName ?? "-"} / {v.Office?.OfficeName ?? "-"}" },
                { "OfficerNameDesignationMobileNo", $"{v.Officer?.OfficerName ?? v.OfficerName ?? "-"} / {v.Designation?.DesignationName ?? "-"}" }
            }).ToList();

            var headers = new List<string>
            {
                "VehicleNoModelNoMfYearChassisNo", "DepartmentNameOfficeDDOCode", "OfficerNameDesignationMobileNo"
            };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
