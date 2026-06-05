using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class GrnDetailsReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "GrnDetails";

        public GrnDetailsReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // Query VehicleCondemnation joined with VehicleInfo to get GRN details
            var query = _context.VehicleCondemnations
                .Include(vc => vc.Vehicle)
                    .ThenInclude(v => v.Department)
                .Include(vc => vc.Vehicle)
                    .ThenInclude(v => v.Office)
                .AsNoTracking()
                .AsQueryable();

            // Optional dept filter
            var deptId = request.Filters.GetInt("deptId");
            if (deptId.HasValue)
            {
                query = query.Where(vc => vc.Vehicle.DeptId == deptId.Value);
            }

            var condemnations = await query
                .OrderBy(vc => vc.Vehicle.VehicleNumber)
                .ToListAsync();

            var data = condemnations.Select(vc => new Dictionary<string, object>
            {
                { "OldVehicleNumber", vc.Vehicle?.VehicleNumber ?? "" },
                { "NewVehicleNumber", vc.ReplacementVehicleRegNo ?? "" },
                { "GrnNumber", vc.GRNNumber ?? "" },
                { "GrnAmount", vc.GRNBillAmount ?? 0 },
                { "GrnDate", vc.GRNDate?.ToString("dd-MM-yyyy") ?? "" },
                { "IsVehicleFullAmount", vc.HaveEnteredAllGRNsFullAmount ? "Yes" : "No" },
                { "AmountSelectedForVehicle", vc.GRNBillAmount ?? 0 },
                { "IsGrnChallanNoOlderThanApril2020", vc.IsOldGrn ? "Yes" : "No" },
                { "IsVerifiedGrn", "Yes" },
                { "GrnFillDate", vc.CreatedDate.ToString("dd-MM-yyyy") },
                { "GrnDoc", !string.IsNullOrEmpty(vc.GrnDocumentPath) ? "View Document" : "N/A" }
            }).ToList();

            var headers = new List<string>
            {
                "OldVehicleNumber", "NewVehicleNumber", "GrnNumber",
                "GrnAmount", "GrnDate", "IsVehicleFullAmount",
                "AmountSelectedForVehicle", "IsGrnChallanNoOlderThanApril2020",
                "IsVerifiedGrn", "GrnFillDate", "GrnDoc"
            };

            return new GenericReportResponseDto
            {
                Title = "Vehicles - GRN Details",
                Headers = headers,
                Data = data
            };
        }
    }
}
