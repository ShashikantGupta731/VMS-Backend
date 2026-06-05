using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs.Reports;
using backend.Models.Core;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Reports
{
    public class FinancialYearExpenditureStrategy : IReportStrategy
    {
        private readonly AppDbContext _context;

        public string ReportType => "FinancialYearExpenditure";

        public FinancialYearExpenditureStrategy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericReportResponseDto> GenerateDataAsync(GenericReportRequestDto request)
        {
            // ── Financial Year Logic ─────────────────────────────────────────────────
            // Indian FY: April → March.  e.g. April 2024 – March 2025 = "FY 2024-25"
            // currentFyStart = April 1st of the most recent April that has already passed
            var today = DateTime.UtcNow;
            var currentFyStartYear = today.Month >= 4 ? today.Year : today.Year - 1;
            var currentFyStart  = new DateTime(currentFyStartYear,      4, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentFyEnd    = new DateTime(currentFyStartYear + 1,  3, 31, 23, 59, 59, DateTimeKind.Utc);
            var prevFyStart     = new DateTime(currentFyStartYear - 1,  4, 1, 0, 0, 0, DateTimeKind.Utc);
            var prevFyEnd       = new DateTime(currentFyStartYear,      3, 31, 23, 59, 59, DateTimeKind.Utc);

            var currentFyLabel = $"{currentFyStartYear}-{(currentFyStartYear + 1) % 100:D2}";
            var prevFyLabel    = $"{currentFyStartYear - 1}-{currentFyStartYear % 100:D2}";

            // ── Optional Dept / Office filters ───────────────────────────────────────
            var deptId   = request.Filters.GetInt("deptId");
            var officeId = request.Filters.GetInt("officeId");

            // Grab matching vehicle IDs once so we can filter both bill tables
            var vehicleQuery = _context.Vehicles.AsNoTracking().AsQueryable();
            if (deptId.HasValue)   vehicleQuery = vehicleQuery.Where(v => v.DeptId   == deptId);
            if (officeId.HasValue) vehicleQuery = vehicleQuery.Where(v => v.OfficeId == officeId);
            var vehicleIds = await vehicleQuery.Select(v => v.VehicleInfoId).ToListAsync();

            // ── Pull Fuel Bills for the two financial years ───────────────────────────
            var fuelBillsRaw = await _context.FuelBills
                .Where(f => vehicleIds.Contains(f.VehicleId)
                         && ((f.BillDate >= currentFyStart && f.BillDate <= currentFyEnd)
                          || (f.BillDate >= prevFyStart    && f.BillDate <= prevFyEnd)))
                .Select(f => new { f.BillDate, f.Amount, f.Status })
                .ToListAsync();

            // ── Pull Maintenance Bills for the two financial years ────────────────────
            var maintBillsRaw = await _context.MaintenanceBills
                .Where(m => vehicleIds.Contains(m.VehicleId)
                         && ((m.BillDate >= currentFyStart && m.BillDate <= currentFyEnd)
                          || (m.BillDate >= prevFyStart    && m.BillDate <= prevFyEnd)))
                .Select(m => new { m.BillDate, m.Amount, m.Status })
                .ToListAsync();

            // ── Month names in Indian FY order (April first) ──────────────────────────
            var monthOrder = new List<(int MonthNum, string MonthName)>
            {
                (4,  "April"), (5,  "May"),      (6,  "June"),
                (7,  "July"),  (8,  "August"),   (9,  "September"),
                (10, "October"), (11, "November"), (12, "December"),
                (1,  "January"), (2,  "February"), (3,  "March")
            };

            // ── Helper: combine fuel + maintenance amounts grouped by month / fy ──────
            // Booked = all bills, Lapsed = Verified bills only
            var allBills = fuelBillsRaw
                .Select(b => new { b.BillDate, b.Amount, b.Status })
                .Concat(maintBillsRaw
                    .Select(b => new { b.BillDate, b.Amount, b.Status }))
                .ToList();

            decimal GetBooked(int monthNum, bool isCurrent)
            {
                var start = isCurrent ? currentFyStart : prevFyStart;
                var end   = isCurrent ? currentFyEnd   : prevFyEnd;
                return allBills
                    .Where(b => b.BillDate.Month == monthNum
                             && b.BillDate >= start
                             && b.BillDate <= end)
                    .Sum(b => b.Amount);
            }

            decimal GetLapsed(int monthNum, bool isCurrent)
            {
                var start = isCurrent ? currentFyStart : prevFyStart;
                var end   = isCurrent ? currentFyEnd   : prevFyEnd;
                return allBills
                    .Where(b => b.BillDate.Month == monthNum
                             && b.BillDate >= start
                             && b.BillDate <= end
                             && b.Status == BillStatus.Verified)
                    .Sum(b => b.Amount);
            }

            // ── Build rows ────────────────────────────────────────────────────────────
            // Values are shown in Crores (÷ 1,00,00,000) rounded to 2 decimal places
            const decimal crore = 10_000_000m;

            var data = new List<Dictionary<string, object>>();
            foreach (var (monthNum, monthName) in monthOrder)
            {
                var bookedPrev    = Math.Round(GetBooked(monthNum, false) / crore, 2);
                var bookedCurr    = Math.Round(GetBooked(monthNum, true)  / crore, 2);
                var bookedDiff    = Math.Round(bookedCurr - bookedPrev, 2);
                var lapsedPrev    = Math.Round(GetLapsed(monthNum, false) / crore, 2);
                var lapsedCurr    = Math.Round(GetLapsed(monthNum, true)  / crore, 2);
                var lapsedDiff    = Math.Round(lapsedCurr - lapsedPrev, 2);

                data.Add(new Dictionary<string, object>
                {
                    { "Month",                   monthName },
                    { $"Booked_{prevFyLabel}",   bookedPrev },
                    { $"Booked_{currentFyLabel}", bookedCurr },
                    { "Booked_Difference",       bookedDiff },
                    { $"Lapsed_{prevFyLabel}",   lapsedPrev },
                    { $"Lapsed_{currentFyLabel}", lapsedCurr },
                    { "Lapsed_Difference",       lapsedDiff }
                });
            }

            // ── Headers ───────────────────────────────────────────────────────────────
            var headers = new List<string>
            {
                "Month",
                $"Booked_{prevFyLabel} (IN.CR)",
                $"Booked_{currentFyLabel} (IN.CR)",
                "Booked_Difference (IN.CR)",
                $"Lapsed_{prevFyLabel} (IN.CR)",
                $"Lapsed_{currentFyLabel} (IN.CR)",
                "Lapsed_Difference (IN.CR)"
            };

            return new GenericReportResponseDto
            {
                Title   = $"Expenditure - Financial Year Wise ({prevFyLabel} vs {currentFyLabel})",
                Headers = headers,
                Data    = data
            };
        }
    }
}
