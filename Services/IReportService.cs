using backend.Models.Core;

namespace backend.Services
{
    public interface IReportService
    {
        Task<IEnumerable<dynamic>> GetAllocationTypeWiseCountAsync(int? distId, int? deptId);
        Task<IEnumerable<dynamic>> GetVoucherTypeBillsAmountAsync(int? distId, int? deptId, DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<dynamic>> GetIncorrectOdometerReadingCountAsync(int? distId, int? deptId);
        Task<IEnumerable<dynamic>> GetUnverifiedVehiclesCountAsync(int? distId, int? deptId);
        Task<IEnumerable<dynamic>> GetVehicleDeptWiseCountAsync(int? deptId);
        Task<IEnumerable<backend.DTOs.Reports.GuestReportResponseDto>> GetPublicGuestRecordsAsync(backend.DTOs.Reports.GuestReportRequestDto request, string guestName, string guestMobileNo, string ipAddress);
        Task<IEnumerable<dynamic>> GetTransportVehiclesAsync();
    }
}
