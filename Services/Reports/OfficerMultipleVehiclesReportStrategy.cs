using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class OfficerMultipleVehiclesReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public OfficerMultipleVehiclesReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "OfficerMultipleVehicles";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // Apply optional filters
            var vehiclesQuery = _context.Vehicles
                .Include(v => v.Department)
                .Include(v => v.Office)
                .Include(v => v.Officer)
                .Include(v => v.Designation)
                .Where(v => v.IsActive && v.CurrentStatus == "Active" && v.OfficerId.HasValue)
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

            // Group by OfficerId locally to find duplicates
            var groupedByOfficer = vehicles
                .GroupBy(v => v.OfficerId.Value)
                .Where(g => g.Count() > 1)
                .ToList();

            var response = new GenericReportResponseDto
            {
                Title = "Officers Mapped to Multiple Active Vehicles",
                Data = new List<Dictionary<string, object>>()
            };

            foreach (var group in groupedByOfficer)
            {
                var sampleVehicle = group.First();
                var officerName = sampleVehicle.Officer?.OfficerName ?? sampleVehicle.OfficerName ?? "Unknown";
                var deptName = sampleVehicle.Department?.DeptName ?? "N/A";
                var designationName = sampleVehicle.Designation?.DesignationName ?? "N/A";

                response.Data.Add(new Dictionary<string, object>
                {
                    { "OfficerName", officerName },
                    { "Designation", designationName },
                    { "DepartmentName", deptName },
                    { "AllocatedVehicles", group.Count() },
                    { "FuelLimit", sampleVehicle.Officer?.FuelLimit ?? 0 },
                    { "FuelLimitSetBy", sampleVehicle.Officer?.FuelLimmitd > 0 ? "Officer" : "Designation" }
                });
            }

            response.Headers = new List<string> { "OfficerName", "Designation", "DepartmentName", "AllocatedVehicles", "FuelLimit", "FuelLimitSetBy" };

            return response;
        }
    }
}
