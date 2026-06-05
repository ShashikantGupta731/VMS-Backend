using System.Threading.Tasks;
using backend.DTOs.Reports;

namespace backend.Services.Reports
{
    public interface IReportStrategy
    {
        string ReportType { get; }
        Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request);
    }
}
