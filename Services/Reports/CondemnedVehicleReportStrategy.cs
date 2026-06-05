using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class CondemnedVehicleReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "CondemnedVehicle";

        public CondemnedVehicleReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var includeNotInUse = request.Filters.GetString("includeNotInUse") == "true";

            var query = _context.Vehicles
                .Include(v => v.Department)
                .Include(v => v.Office)
                    .ThenInclude(o => o.District)
                .AsNoTracking()
                .AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(v => v.DeptId == deptId);
            }

            var officeId = request.Filters.GetInt("officeId");
            if (officeId.HasValue) {
                query = query.Where(v => v.OfficeId == officeId);
            }

            if (includeNotInUse)
            {
                // Report 10: Vehicles - Not In Use / Condemned
                query = query.Where(v => v.CurrentStatus == "Not In Use" || v.CurrentStatus == "NotInUse" || v.CurrentStatus == "Condemned");

                var groupedData = await query
                    .GroupBy(v => v.OfficeId)
                    .Select(g => new
                    {
                        Department = g.FirstOrDefault()!.Department != null ? g.FirstOrDefault()!.Department!.DeptName : "Unknown",
                        District = g.FirstOrDefault()!.Office != null && g.FirstOrDefault()!.Office!.District != null ? g.FirstOrDefault()!.Office!.District!.DistrictName : "Unknown",
                        Office = g.FirstOrDefault()!.Office != null ? g.FirstOrDefault()!.Office!.OfficeName : "Unknown",
                        DDONameCode = g.FirstOrDefault()!.Office != null ? g.FirstOrDefault()!.Office!.OfficeAbbreviation : "Unknown",
                        NoOfVehiclesNotInUse = g.Count(v => v.CurrentStatus == "Not In Use" || v.CurrentStatus == "NotInUse"),
                        NoOfVehiclesCondemned = g.Count(v => v.CurrentStatus == "Condemned")
                    })
                    .ToListAsync();

                var data = groupedData.Select(g => new Dictionary<string, object>
                {
                    { "Department", g.Department },
                    { "District", g.District },
                    { "Office", g.Office },
                    { "DDONameCode", g.DDONameCode },
                    { "NoOfVehiclesNotInUse", g.NoOfVehiclesNotInUse },
                    { "NoOfVehiclesCondemned", g.NoOfVehiclesCondemned }
                }).ToList();

                return new GenericReportResponseDto
                {
                    Headers = new List<string> { "Department", "District", "Office", "DDONameCode", "NoOfVehiclesNotInUse", "NoOfVehiclesCondemned" },
                    Data = data
                };
            }
            else
            {
                // Report 22: Vehicles - Condemned
                query = query.Where(v => v.CurrentStatus == "Condemned");

                var groupedData = await query
                    .GroupBy(v => v.DeptId)
                    .Select(g => new
                    {
                        DepartmentName = g.FirstOrDefault()!.Department != null ? g.FirstOrDefault()!.Department!.DeptName : "Unknown",
                        NoOfCondemnedVehicles = g.Count()
                    })
                    .ToListAsync();

                var data = groupedData.Select(g => new Dictionary<string, object>
                {
                    { "DepartmentName", g.DepartmentName },
                    { "NoOfCondemnedVehicles", g.NoOfCondemnedVehicles }
                }).ToList();

                return new GenericReportResponseDto
                {
                    Headers = new List<string> { "DepartmentName", "NoOfCondemnedVehicles" },
                    Data = data
                };
            }
        }
    }
}
