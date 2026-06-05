using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public class PublicGuestRecordsReportStrategy : IReportStrategy
    {
        public string ReportType => "PublicGuestRecords";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // Note: The specific Public Guest Log table is currently not defined in EF Core models.
            // Returning an empty structure to satisfy the strategy pattern and API contract for now.
            // This needs to be populated once the PublicGuestRecords entity is scaffolded.
            
            var headers = new List<string>
            {
                "VehicleInfoId", "Department", "GuestName", "UsageDateFrom", "UsageDateTo"
            };

            var data = new List<Dictionary<string, object>>();

            return await Task.FromResult(new GenericReportResponseDto
            {
                Headers = headers,
                Data = data
            });
        }
    }
}
