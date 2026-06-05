using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class VerifiedUnverifiedVehicleReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "VerifiedUnverifiedVehicle";

        public VerifiedUnverifiedVehicleReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Vehicles
                .Include(v => v.Department)
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
            
            var statusStr = request.Filters.GetString("status");

            if (statusStr == "Verified")
            {
                var groupedData = await query
                    .GroupBy(v => v.DeptId)
                    .Select(g => new
                    {
                        DepartmentName = g.FirstOrDefault()!.Department != null ? g.FirstOrDefault()!.Department!.DeptName : "Unknown",
                        EnrolledVehicles = g.Count(),
                        VerifiedVehicles = g.Count(v => v.verificationstatus == 1)
                    })
                    .ToListAsync();

                var data = groupedData.Select(g => new Dictionary<string, object>
                {
                    { "DepartmentName", g.DepartmentName },
                    { "EnrolledVehicles", g.EnrolledVehicles },
                    { "VerifiedVehicles", g.VerifiedVehicles }
                }).ToList();

                return new GenericReportResponseDto
                {
                    Headers = new List<string> { "DepartmentName", "EnrolledVehicles", "VerifiedVehicles" },
                    Data = data
                };
            }
            else // Unverified
            {
                // Filter specifically for unverified
                query = query.Where(v => v.verificationstatus == 0 || v.verificationstatus == null);

                var groupedData = await query
                    .GroupBy(v => v.DeptId)
                    .Select(g => new
                    {
                        DepartmentName = g.FirstOrDefault()!.Department != null ? g.FirstOrDefault()!.Department!.DeptName : "Unknown",
                        NoOfVehicles = g.Count()
                    })
                    .ToListAsync();

                var data = groupedData.Select(g => new Dictionary<string, object>
                {
                    { "DepartmentName", g.DepartmentName },
                    { "NoOfVehicles", g.NoOfVehicles }
                }).ToList();

                return new GenericReportResponseDto
                {
                    Headers = new List<string> { "DepartmentName", "NoOfVehicles" },
                    Data = data
                };
            }
        }
    }
}
