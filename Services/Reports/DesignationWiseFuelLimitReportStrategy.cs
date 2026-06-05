using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class DesignationWiseFuelLimitReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "DesignationWiseFuelLimit";

        public DesignationWiseFuelLimitReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Designations
                .Include(d => d.Department)
                .AsNoTracking()
                .AsQueryable();

            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue) {
                query = query.Where(d => d.DeptId == deptId);
            }
            var desig = request.Filters.GetString("designationName");
            if (!string.IsNullOrEmpty(desig)) {
                string designationName = desig;
                query = query.Where(d => d.DesignationName.Contains(designationName));
            }

            var designations = await query.ToListAsync();

            var data = designations.Select(d => new Dictionary<string, object>
            {
                { "DesignationName", d.DesignationName },
                { "DepartmentName", d.Department?.DeptName ?? "" },
                { "PetrolFuelLimit", d.PetrolFuelLimit },
                { "DieselFuelLimit", d.DieselFuelLimit },
                { "MaintenanceLimitRs", d.PetrolMaintenanceLimit }
            }).ToList();

            var headers = new List<string>
            {
                "DesignationName", "DepartmentName", 
                "PetrolFuelLimit", "DieselFuelLimit", "MaintenanceLimitRs"
            };

            return new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
