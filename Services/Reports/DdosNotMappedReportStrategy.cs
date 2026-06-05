using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class DdosNotMappedReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public DdosNotMappedReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "DdosNotMapped";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // Apply optional filters
            var vehiclesQuery = _context.Vehicles
                .Include(v => v.Department)
                .Include(v => v.Office)
                .ThenInclude(o => o.District)
                .Where(v => v.IsActive && v.CurrentStatus == "Active" && !string.IsNullOrEmpty(v.DDOId))
                .AsQueryable();

            var deptId = request.Filters.GetInt("deptId"); // also captures departmentId implicitly
            if (deptId.HasValue) {
                vehiclesQuery = vehiclesQuery.Where(v => v.DeptId == deptId);
            }
            var distId = request.Filters.GetInt("districtId");
            if (distId.HasValue) {
                vehiclesQuery = vehiclesQuery.Where(v => v.Office.DistrictId == distId);
            }

            var vehicles = await vehiclesQuery.ToListAsync();

            // Group by DDOId to find which DDO codes don't have Nodal Officer details set in their vehicles
            var groupedByDdo = vehicles
                .GroupBy(v => v.DDOId)
                .Where(g => g.Any(v => string.IsNullOrEmpty(v.NodalOfficerUsername)))
                .ToList();

            var response = new GenericReportResponseDto
            {
                Title = "DDOs Not Mapped to a Nodal Officer",
                Data = new List<Dictionary<string, object>>()
            };

            foreach (var group in groupedByDdo)
            {
                var sampleVehicle = group.First();
                var officeName = sampleVehicle.Office?.OfficeName ?? "N/A";
                var districtName = sampleVehicle.Office?.District?.DistrictName ?? "N/A";

                response.Data.Add(new Dictionary<string, object>
                {
                    { "District", districtName },
                    { "OfficeName", officeName },
                    { "DDOCode", group.Key }
                });
            }

            response.Headers = new List<string> { "District", "OfficeName", "DDOCode" };

            return response;
        }
    }
}
