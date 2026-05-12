using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<dynamic>> GetAllocationTypeWiseCountAsync(int? distId, int? deptId)
        {
            var query = _context.Vehicles.AsQueryable();

            if (distId.HasValue) query = query.Where(v => v.Office.DistrictId == distId);
            if (deptId.HasValue) query = query.Where(v => v.DeptId == deptId);

            return await query
                .GroupBy(v => v.VehicleType!.VehicleTypeName)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync<dynamic>();
        }

        public async Task<IEnumerable<dynamic>> GetVoucherTypeBillsAmountAsync(int? distId, int? deptId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.BillClaims.AsQueryable();

            if (distId.HasValue) query = query.Where(b => b.Vehicle!.Office.DistrictId == distId);
            if (deptId.HasValue) query = query.Where(b => b.Vehicle!.DeptId == deptId);
            if (fromDate.HasValue) query = query.Where(b => b.CreatedAt >= fromDate);
            if (toDate.HasValue) query = query.Where(b => b.CreatedAt <= toDate);

            return await query
                .GroupBy(b => b.Type)
                .Select(g => new
                {
                    VoucherType = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(b => b.TotalAmount)
                })
                .ToListAsync<dynamic>();
        }

        public async Task<IEnumerable<dynamic>> GetIncorrectOdometerReadingCountAsync(int? distId, int? deptId)
        {
            // Placeholder: Use fuel as a proxy or just return 0 issues for now
            return await _context.Vehicles
                .Where(v => string.IsNullOrEmpty(v.VehicleNumber))
                .GroupBy(v => v.Office.District.DistrictName)
                .Select(g => new { District = g.Key, Issues = g.Count() })
                .ToListAsync<dynamic>();
        }

        public async Task<IEnumerable<dynamic>> GetUnverifiedVehiclesCountAsync(int? distId, int? deptId)
        {
            var query = _context.Vehicles.Where(v => v.verificationstatus != 1);

            if (distId.HasValue) query = query.Where(v => v.Office.DistrictId == distId);
            if (deptId.HasValue) query = query.Where(v => v.DeptId == deptId);

            return await query
                .GroupBy(v => v.Department!.DeptName)
                .Select(g => new { Department = g.Key, UnverifiedCount = g.Count() })
                .ToListAsync<dynamic>();
        }

        public async Task<IEnumerable<dynamic>> GetVehicleDeptWiseCountAsync(int? deptId)
        {
            var query = _context.Vehicles.AsQueryable();
            if (deptId.HasValue) query = query.Where(v => v.DeptId == deptId);

            return await query
                .GroupBy(v => v.Department!.DeptName)
                .Select(g => new { Department = g.Key, TotalVehicles = g.Count() })
                .ToListAsync<dynamic>();
        }
    }
}
