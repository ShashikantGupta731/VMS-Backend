using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Reports;
using backend.Models.Core;

namespace backend.Services.Reports
{
    public class VoucherTypeBillsReportStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public VoucherTypeBillsReportStrategy(AppDbContext context)
        {
            _context = context;
        }

        public string ReportType => "VoucherTypeBills";

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            var deptId = request.Filters.GetInt("deptId");
            var officeId = request.Filters.GetInt("officeId");

            // Fuel Bills
            var fuelQuery = _context.FuelBills.Include(b => b.Vehicle).AsNoTracking().AsQueryable();
            if (deptId.HasValue) fuelQuery = fuelQuery.Where(b => b.Vehicle.DeptId == deptId);
            if (officeId.HasValue) fuelQuery = fuelQuery.Where(b => b.Vehicle.OfficeId == officeId);
            var fuelBills = await fuelQuery.ToListAsync();

            // Maintenance Bills
            var maintQuery = _context.MaintenanceBills.Include(b => b.Vehicle).AsNoTracking().AsQueryable();
            if (deptId.HasValue) maintQuery = maintQuery.Where(b => b.Vehicle.DeptId == deptId);
            if (officeId.HasValue) maintQuery = maintQuery.Where(b => b.Vehicle.OfficeId == officeId);
            var maintBills = await maintQuery.ToListAsync();

            // Miscellaneous Bills (no direct link to Dept/Office, so we get all or none)
            var miscBills = new List<MiscellaneousBill>();
            if (!deptId.HasValue && !officeId.HasValue) 
            {
                miscBills = await _context.MiscellaneousBills.AsNoTracking().ToListAsync();
            }

            // Hired Vehicle Bills (no direct link to Dept/Office, so we get all or none)
            var hiredBills = new List<HiredVehicleBill>();
            if (!deptId.HasValue && !officeId.HasValue)
            {
                hiredBills = await _context.HiredVehicleBills.AsNoTracking().ToListAsync();
            }

            var response = new GenericReportResponseDto
            {
                Title = "Voucher Type Bills (Counts & Amounts)",
                Headers = new List<string> { "VoucherType", "NoOfBills", "TotalAmountInRs" },
                Data = new List<Dictionary<string, object>>()
            };

            response.Data.Add(new Dictionary<string, object>
            {
                { "VoucherType", "Fuel Bills" },
                { "NoOfBills", fuelBills.Count },
                { "TotalAmountInRs", fuelBills.Sum(b => b.Amount) }
            });

            response.Data.Add(new Dictionary<string, object>
            {
                { "VoucherType", "Maintenance Bills" },
                { "NoOfBills", maintBills.Count },
                { "TotalAmountInRs", maintBills.Sum(b => b.Amount) }
            });

            response.Data.Add(new Dictionary<string, object>
            {
                { "VoucherType", "Miscellaneous Bills" },
                { "NoOfBills", miscBills.Count },
                { "TotalAmountInRs", miscBills.Sum(b => b.Amount) }
            });

            response.Data.Add(new Dictionary<string, object>
            {
                { "VoucherType", "Hired Vehicle Bills" },
                { "NoOfBills", hiredBills.Count },
                { "TotalAmountInRs", hiredBills.Sum(b => b.Amount) }
            });

            return response;
        }
    }
}
