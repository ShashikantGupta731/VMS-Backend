using backend.DTOs.Reports;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class VehicleModelReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public VehicleModelReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "VehicleModelData";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.VehicleModels
                .Include(m => m.Manufacturer)
                .AsNoTracking()
                .AsQueryable();

            var manufacturerId = request.Filters.GetInt("manufacturerId");
            if (manufacturerId.HasValue)
            {
                query = query.Where(m => m.ManufacturerId == manufacturerId.Value);
            }

            var deptId = request.Filters.GetInt("deptId");
            var districtId = request.Filters.GetInt("districtId");

            var groupedQuery = query.Select(m => new
                {
                    Manufacturer = m.Manufacturer != null ? m.Manufacturer.ManufacturerName : "Unknown",
                    Model = m.ModelName,
                    TotalVehicles = _context.Vehicles.Count(v => 
                        v.IsActive && 
                        v.ModelId == m.ModelId &&
                        (!deptId.HasValue || v.DeptId == deptId.Value) &&
                        (!districtId.HasValue || (v.Office != null && v.Office.DistrictId == districtId.Value))
                    )
                })
                .Where(m => m.TotalVehicles > 0)
                .OrderByDescending(m => m.TotalVehicles);

            var result = await groupedQuery.ToListAsync();

            var data = result.Select(m => new Dictionary<string, object>
            {
                { "ModelName", m.Model },
                { "NoOfVehicles", m.TotalVehicles }
            }).ToList();

            var headers = new List<string>
            {
                "ModelName",
                "NoOfVehicles"
            };

            return new GenericReportResponseDto
            {
                Title = "Model-wise Vehicle Report",
                Headers = headers,
                Data = data
            };
        }
    }
}
