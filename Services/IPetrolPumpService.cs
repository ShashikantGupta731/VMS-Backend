using backend.DTOs.PetrolPump;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IPetrolPumpService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(int userId);
        Task<List<FuelLogDto>> GetRecentFuelLogsAsync(int userId);
        Task<StockAmountDto> GetStockAmountsAsync(int userId);
        Task<bool> InsertFuelEntryAsync(int userId, FuelEntryRequestDto request);
        Task<List<VehicleSearchDto>> SearchVehiclesAsync(string searchQuery);
    }
}
