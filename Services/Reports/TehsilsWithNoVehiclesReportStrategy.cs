using backend.DTOs.Reports;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class TehsilsWithNoVehiclesReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public TehsilsWithNoVehiclesReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "TehsilsWithNoVehicles";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var query = _context.Tehsils
                .Include(t => t.District)
                .Where(t => t.IsActive && 
                            !_context.Vehicles.Any(v => v.IsActive && v.Office.TehsilId == t.TehsilId))
                .Select(t => new
                {
                    District = t.District.DistrictName,
                    Tehsil = t.TehsilName,
                    Status = t.IsActive ? "Active" : "Inactive"
                });

            var result = await query.ToListAsync();

            var data = result.Select(t => new Dictionary<string, object>
            {
                { "District", t.District },
                { "Tehsil", t.Tehsil }
            }).ToList();

            var headers = new List<string>
            {
                "District",
                "Tehsil"
            };

            return new GenericReportResponseDto
            {
                Title = "Tehsils with No Vehicles",
                Headers = headers,
                Data = data
            };
        }
    }
}
