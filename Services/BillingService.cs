using backend.Data;
using backend.DTOs.Billing;
using backend.Models.Core;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class BillingService : IBillingService
    {
        private readonly AppDbContext _context;

        public BillingService(AppDbContext context)
        {
            _context = context;
        }

        #region Fuel Bills

        public async Task<List<FuelBillResponseDto>> GetFuelBillsAsync(int userId, int? claimId = null)
        {
            var query = _context.FuelBills
                .Include(f => f.Vehicle)
                .Include(f => f.Claim)
                .Where(f => f.CreatedById == userId);

            if (claimId.HasValue)
                query = query.Where(f => f.ClaimId == claimId);
            else
                query = query.Where(f => f.Status == BillStatus.Draft);

            return await query.Select(f => new FuelBillResponseDto
            {
                FuelBillId = f.FuelBillId,
                VehicleId = f.VehicleId,
                VehicleNumber = f.Vehicle.VehicleNumber,
                BillNumber = f.BillNumber,
                BillDate = f.BillDate,
                OdometerReading = f.OdometerReading,
                FuelQuantity = f.FuelQuantity,
                Amount = f.Amount,
                Status = f.Status,
                ClaimId = f.ClaimId,
                ClaimNumber = f.Claim != null ? f.Claim.ClaimNumber : null
            }).ToListAsync();
        }

        public async Task<FuelBillResponseDto> SaveFuelBillAsync(CreateFuelBillDto dto, int userId)
        {
            var validation = await GetOdometerValidationAsync(dto.VehicleId, dto.BillDate);
            if (dto.OdometerReading <= validation.LastReading)
            {
                throw new Exception($"Odometer reading must be greater than the last reading ({validation.LastReading})");
            }

            bool isNew = !dto.FuelBillId.HasValue || dto.FuelBillId <= 0;
            FuelBill bill;
            if (!isNew)
            {
                bill = await _context.FuelBills.FindAsync(dto.FuelBillId) ?? throw new Exception("Bill not found");
                if (bill.CreatedById != userId) throw new UnauthorizedAccessException();
            }
            else
            {
                bill = new FuelBill { CreatedById = userId, CreatedAt = DateTime.UtcNow };
            }

            bill.VehicleId = dto.VehicleId;
            bill.BillNumber = dto.BillNumber;
            bill.BillDate = dto.BillDate;
            bill.OdometerReading = dto.OdometerReading;
            bill.FuelQuantity = dto.FuelQuantity;
            bill.Amount = dto.Amount;
            bill.NocFile = dto.NocFile;
            bill.NocIssueDate = dto.NocIssueDate.HasValue ? DateTime.SpecifyKind(dto.NocIssueDate.Value, DateTimeKind.Utc) : null;
            bill.NocExpiryDate = dto.NocExpiryDate.HasValue ? DateTime.SpecifyKind(dto.NocExpiryDate.Value, DateTimeKind.Utc) : null;
            bill.Status = BillStatus.Draft;

            await _context.SaveChangesAsync();

            return new FuelBillResponseDto
            {
                FuelBillId = bill.FuelBillId,
                VehicleId = bill.VehicleId,
                BillNumber = bill.BillNumber,
                BillDate = bill.BillDate,
                OdometerReading = bill.OdometerReading,
                FuelQuantity = bill.FuelQuantity,
                Amount = bill.Amount,
                Status = bill.Status
            };
        }

        public async Task<bool> DeleteFuelBillAsync(int id, int userId)
        {
            var bill = await _context.FuelBills.FindAsync(id);
            if (bill == null || bill.CreatedById != userId || bill.Status != BillStatus.Draft)
                return false;

            _context.FuelBills.Remove(bill);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Maintenance Bills

        public async Task<List<MaintenanceBillResponseDto>> GetMaintenanceBillsAsync(int userId, int? claimId = null)
        {
            var query = _context.MaintenanceBills
                .Include(m => m.Vehicle)
                .Where(m => m.CreatedById == userId);

            if (claimId.HasValue)
                query = query.Where(m => m.ClaimId == claimId);
            else
                query = query.Where(m => m.Status == BillStatus.Draft);

            return await query.Select(m => new MaintenanceBillResponseDto
            {
                MaintenanceBillId = m.MaintenanceBillId,
                VehicleId = m.VehicleId,
                VehicleNumber = m.Vehicle.VehicleNumber,
                BillNumber = m.BillNumber,
                BillDate = m.BillDate,
                OdometerReading = m.OdometerReading,
                Amount = m.Amount,
                MaintenanceType = m.MaintenanceType,
                Details = m.Details,
                Status = m.Status,
                ClaimId = m.ClaimId
            }).ToListAsync();
        }

        public async Task<MaintenanceBillResponseDto> SaveMaintenanceBillAsync(CreateMaintenanceBillDto dto, int userId)
        {
             // Odometer Validation
            var validation = await GetOdometerValidationAsync(dto.VehicleId, dto.BillDate);
            if (dto.OdometerReading <= validation.LastReading)
            {
                throw new Exception($"Odometer reading must be greater than the last reading ({validation.LastReading})");
            }

            bool isNew = !dto.MaintenanceBillId.HasValue || dto.MaintenanceBillId <= 0;
            MaintenanceBill bill;
            if (!isNew)
            {
                bill = await _context.MaintenanceBills.FindAsync(dto.MaintenanceBillId) ?? throw new Exception("Bill not found");
                if (bill.CreatedById != userId) throw new UnauthorizedAccessException();
                if (bill.Status != BillStatus.Draft) throw new Exception("Only draft bills can be modified");
            }
            else
            {
                bill = new MaintenanceBill { CreatedById = userId, CreatedAt = DateTime.UtcNow };
            }

            bill.VehicleId = dto.VehicleId;
            bill.BillNumber = dto.BillNumber;
            bill.BillDate = dto.BillDate;
            bill.OdometerReading = dto.OdometerReading;
            bill.Amount = dto.Amount;
            bill.MaintenanceType = dto.MaintenanceType;
            bill.Details = dto.Details;
            bill.SanctionPermissionFile = dto.SanctionPermissionFile;
            bill.Status = BillStatus.Draft;

            if (isNew) _context.MaintenanceBills.Add(bill);
            await _context.SaveChangesAsync();
            return new MaintenanceBillResponseDto
            {
                MaintenanceBillId = bill.MaintenanceBillId,
                VehicleId = bill.VehicleId,
                BillNumber = bill.BillNumber,
                BillDate = bill.BillDate,
                OdometerReading = bill.OdometerReading,
                Amount = bill.Amount,
                MaintenanceType = bill.MaintenanceType,
                Status = bill.Status
            };
        }

        public async Task<bool> DeleteMaintenanceBillAsync(int id, int userId)
        {
            var bill = await _context.MaintenanceBills.FindAsync(id);
            if (bill == null || bill.CreatedById != userId || bill.Status != BillStatus.Draft)
                return false;

            _context.MaintenanceBills.Remove(bill);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Claims

        public async Task<BillClaimResponseDto> CreateClaimAsync(CreateClaimDto dto, int userId)
        {
            var claim = new BillClaim
            {
                ClaimNumber = $"CLM-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                Type = dto.Type,
                Status = BillStatus.Pending,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 0,
                ForwardedToTreasury = dto.ForwardedToTreasury,
                SubVoucherNo = dto.SubVoucherNo,
                SubVoucherDescription = dto.SubVoucherDescription,
                SanctionOrderNo = dto.SanctionOrderNo,
                SanctionOrderDate = dto.SanctionOrderDate,
                SanctionAuthority = dto.SanctionAuthority,
                FirmName = dto.FirmName,
                Tax = dto.Tax
            };

            if (dto.Type == BillType.Fuel)
            {
                var bills = await _context.FuelBills.Where(b => dto.BillIds.Contains(b.FuelBillId) && b.Status == BillStatus.Draft).ToListAsync();
                if (bills.Any()) claim.VehicleId = bills.First().VehicleId;
                foreach (var b in bills)
                {
                    b.Status = BillStatus.Pending;
                    b.Claim = claim;
                    claim.TotalAmount += b.Amount;
                }
            }
            else if (dto.Type == BillType.Maintenance)
            {
                var bills = await _context.MaintenanceBills.Where(b => dto.BillIds.Contains(b.MaintenanceBillId) && b.Status == BillStatus.Draft).ToListAsync();
                if (bills.Any()) claim.VehicleId = bills.First().VehicleId;
                foreach (var b in bills)
                {
                    b.Status = BillStatus.Pending;
                    b.Claim = claim;
                    claim.TotalAmount += b.Amount;
                }
            }
            else if (dto.Type == BillType.Hired)
            {
                var bills = await _context.HiredVehicleBills.Where(b => dto.BillIds.Contains(b.HiredVehicleBillId) && b.Status == BillStatus.Draft).ToListAsync();
                foreach (var b in bills)
                {
                    b.Status = BillStatus.Pending;
                    b.Claim = claim;
                    claim.TotalAmount += b.Amount;
                }
            }
            else if (dto.Type == BillType.Contractual)
            {
                var bills = await _context.ContractualBills.Where(b => dto.BillIds.Contains(b.ContractualBillId) && b.Status == BillStatus.Draft).ToListAsync();
                foreach (var b in bills)
                {
                    b.Status = BillStatus.Pending;
                    b.Claim = claim;
                    claim.TotalAmount += b.Amount;
                }
            }

            _context.BillClaims.Add(claim);
            await _context.SaveChangesAsync();

            return new BillClaimResponseDto
            {
                BillClaimId = claim.BillClaimId,
                ClaimNumber = claim.ClaimNumber,
                TotalAmount = claim.TotalAmount,
                Status = claim.Status,
                Type = claim.Type,
                CreatedAt = claim.CreatedAt
            };
        }

        public async Task<List<BillClaimResponseDto>> GetClaimsAsync(int userId, string role)
        {
            IQueryable<BillClaim> query = _context.BillClaims.Include(c => c.CreatedBy);

            if (role == "DDO")
                query = query.Where(c => c.CreatedById == userId);
            else if (role == "ADMN" || role == "NDOF")
                query = query.Where(c => c.Status == BillStatus.Pending);

            return await query.Select(c => new BillClaimResponseDto
            {
                BillClaimId = c.BillClaimId,
                ClaimNumber = c.ClaimNumber,
                TotalAmount = c.TotalAmount,
                Status = c.Status,
                Type = c.Type,
                CreatedBy = $"{c.CreatedBy.FirstName} {c.CreatedBy.LastName}",
                CreatedAt = c.CreatedAt,
                Comments = c.Comments
            }).ToListAsync();
        }

        public async Task<BillClaimDetailDto?> GetClaimByIdAsync(int id)
        {
            var claim = await _context.BillClaims
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(c => c.BillClaimId == id);

            if (claim == null) return null;

            var dto = new BillClaimDetailDto
            {
                BillClaimId = claim.BillClaimId,
                ClaimNumber = claim.ClaimNumber,
                TotalAmount = claim.TotalAmount,
                Status = claim.Status,
                Type = claim.Type,
                CreatedBy = $"{claim.CreatedBy.FirstName} {claim.CreatedBy.LastName}",
                CreatedAt = claim.CreatedAt,
                Comments = claim.Comments,
                ForwardedToTreasury = claim.ForwardedToTreasury,
                SubVoucherNo = claim.SubVoucherNo,
                SubVoucherDescription = claim.SubVoucherDescription,
                SanctionOrderNo = claim.SanctionOrderNo,
                SanctionOrderDate = claim.SanctionOrderDate,
                SanctionAuthority = claim.SanctionAuthority,
                FirmName = claim.FirmName,
                Tax = claim.Tax
            };

            if (claim.Type == BillType.Fuel)
            {
                dto.FuelBills = await _context.FuelBills
                    .Include(b => b.Vehicle)
                    .Where(b => b.ClaimId == claim.BillClaimId)
                    .Select(b => new FuelBillResponseDto
                    {
                        FuelBillId = b.FuelBillId,
                        VehicleId = b.VehicleId,
                        VehicleNumber = b.Vehicle.VehicleNumber,
                        BillNumber = b.BillNumber,
                        BillDate = b.BillDate,
                        OdometerReading = b.OdometerReading,
                        FuelQuantity = b.FuelQuantity,
                        Amount = b.Amount,
                        Status = b.Status
                    }).ToListAsync();
            }
            else if (claim.Type == BillType.Maintenance)
            {
                dto.MaintenanceBills = await _context.MaintenanceBills
                    .Include(b => b.Vehicle)
                    .Where(b => b.ClaimId == claim.BillClaimId)
                    .Select(b => new MaintenanceBillResponseDto
                    {
                        MaintenanceBillId = b.MaintenanceBillId,
                        VehicleId = b.VehicleId,
                        VehicleNumber = b.Vehicle.VehicleNumber,
                        BillNumber = b.BillNumber,
                        BillDate = b.BillDate,
                        OdometerReading = b.OdometerReading,
                        Amount = b.Amount,
                        MaintenanceType = b.MaintenanceType,
                        Status = b.Status
                    }).ToListAsync();
            }
            else if (claim.Type == BillType.Hired)
            {
                dto.HiredVehicleBills = await _context.HiredVehicleBills
                    .Where(b => b.ClaimId == claim.BillClaimId)
                    .Select(b => new HiredVehicleBillResponseDto
                    {
                        HiredVehicleBillId = b.HiredVehicleBillId,
                        BillNumber = b.BillNumber,
                        BillDate = b.BillDate,
                        VehicleNumber = b.VehicleNumber,
                        Amount = b.Amount,
                        Status = b.Status
                    }).ToListAsync();
            }
            else if (claim.Type == BillType.Contractual)
            {
                dto.ContractualBills = await _context.ContractualBills
                    .Where(b => b.ClaimId == claim.BillClaimId)
                    .Select(b => new ContractualBillResponseDto
                    {
                        ContractualBillId = b.ContractualBillId,
                        BillNumber = b.BillNumber,
                        BillDate = b.BillDate,
                        VehicleNumber = b.VehicleNumber,
                        Amount = b.Amount,
                        Status = b.Status
                    }).ToListAsync();
            }

            return dto;
        }

        public async Task<bool> VerifyClaimAsync(int claimId, int verifierId, string? comments)
        {
            var claim = await _context.BillClaims.FindAsync(claimId);
            if (claim == null || claim.Status != BillStatus.Pending) return false;

            claim.Status = BillStatus.Verified;
            claim.VerifiedById = verifierId;
            claim.VerifiedAt = DateTime.UtcNow;
            claim.Comments = comments;

            // Update individual bills
            var fuelBills = await _context.FuelBills.Where(b => b.ClaimId == claimId).ToListAsync();
            fuelBills.ForEach(b => b.Status = BillStatus.Verified);

            var maintBills = await _context.MaintenanceBills.Where(b => b.ClaimId == claimId).ToListAsync();
            maintBills.ForEach(b => b.Status = BillStatus.Verified);

            var hiredBills = await _context.HiredVehicleBills.Where(b => b.ClaimId == claimId).ToListAsync();
            hiredBills.ForEach(b => b.Status = BillStatus.Verified);

            var contractBills = await _context.ContractualBills.Where(b => b.ClaimId == claimId).ToListAsync();
            contractBills.ForEach(b => b.Status = BillStatus.Verified);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectClaimAsync(int claimId, int verifierId, string comments)
        {
            var claim = await _context.BillClaims.FindAsync(claimId);
            if (claim == null || claim.Status != BillStatus.Pending) return false;

            claim.Status = BillStatus.Rejected;
            claim.VerifiedById = verifierId;
            claim.VerifiedAt = DateTime.UtcNow;
            claim.Comments = comments;

            // Revert individual bills back to Draft so DDO can fix them
            var fuelBills = await _context.FuelBills.Where(b => b.ClaimId == claimId).ToListAsync();
            foreach(var b in fuelBills) { b.Status = BillStatus.Draft; b.ClaimId = null; }

            var maintBills = await _context.MaintenanceBills.Where(b => b.ClaimId == claimId).ToListAsync();
            foreach(var b in maintBills) { b.Status = BillStatus.Draft; b.ClaimId = null; }

            var hiredBills = await _context.HiredVehicleBills.Where(b => b.ClaimId == claimId).ToListAsync();
            foreach(var b in hiredBills) { b.Status = BillStatus.Draft; b.ClaimId = null; }

            var contractBills = await _context.ContractualBills.Where(b => b.ClaimId == claimId).ToListAsync();
            foreach(var b in contractBills) { b.Status = BillStatus.Draft; b.ClaimId = null; }

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        public async Task<OdometerValidationDto> GetOdometerValidationAsync(int vehicleId, DateTime billDate)
        {
            // Get last verified fuel bill reading
            var lastFuel = await _context.FuelBills
                .Where(f => f.VehicleId == vehicleId && f.Status == BillStatus.Verified)
                .OrderByDescending(f => f.OdometerReading)
                .FirstOrDefaultAsync();

            // Get last verified maintenance bill reading
            var lastMaint = await _context.MaintenanceBills
                .Where(m => m.VehicleId == vehicleId && m.Status == BillStatus.Verified)
                .OrderByDescending(m => m.OdometerReading)
                .FirstOrDefaultAsync();

            int fuelReading = lastFuel?.OdometerReading ?? 0;
            int maintReading = lastMaint?.OdometerReading ?? 0;
            
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            int initialReading = 0; 

            int maxReading = Math.Max(fuelReading, Math.Max(maintReading, initialReading));
            DateTime? lastDate = fuelReading > maintReading ? lastFuel?.BillDate : lastMaint?.BillDate;

            return new OdometerValidationDto
            {
                LastReading = maxReading,
                LastReadingDate = lastDate,
                IsValid = true 
            };
        }

        public async Task<bool> CheckFitnessRequirementAsync(int vehicleId, int currentOdometer)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null) return false;

            // 1. Check Mileage (> 250,000 KM)
            if (currentOdometer > 250000) return true;

            // 2. Check Age (> 15 Years)
            int currentYear = DateTime.Now.Year;
            if (vehicle.ManufactureYear > 0 && (currentYear - vehicle.ManufactureYear) > 15) return true;
            if (vehicle.VehiclePurchaseDate.HasValue && (currentYear - vehicle.VehiclePurchaseDate.Value.Year) > 15) return true;

            return false;
        }

        public async Task<bool> CheckPermissionRequirementAsync(int vehicleId, BillType type, decimal amountOrLitres)
        {
            // Hardcoded thresholds (To be moved to a master table later)
            if (type == BillType.Fuel)
            {
                // Example: If liters > 100 in a single receipt (Department dependent usually)
                return amountOrLitres > 100;
            }
            else if (type == BillType.Maintenance)
            {
                // Example: If amount > 5000 (standard limit)
                return amountOrLitres > 5000;
            }

            return false;
        }
        #region Hired Vehicle Bills

        public async Task<List<HiredVehicleBillResponseDto>> GetHiredVehicleBillsAsync(int userId, int? claimId = null)
        {
            var query = _context.HiredVehicleBills
                .Include(f => f.Claim)
                .Where(f => f.CreatedById == userId);

            if (claimId.HasValue)
                query = query.Where(f => f.ClaimId == claimId);
            else
                query = query.Where(f => f.Status == BillStatus.Draft);

            return await query.Select(f => new HiredVehicleBillResponseDto
            {
                HiredVehicleBillId = f.HiredVehicleBillId,
                BillNumber = f.BillNumber,
                BillDate = f.BillDate,
                VehicleNumber = f.VehicleNumber,
                OfficeName = f.OfficeName,
                ContractorName = f.ContractorName,
                ContractorPhone = f.ContractorPhone,
                VehicleType = f.VehicleType,
                NoOfVehicles = f.NoOfVehicles,
                HiredFrom = f.HiredFrom,
                HiredTo = f.HiredTo,
                KmCovered = f.KmCovered,
                Amount = f.Amount,
                Status = f.Status,
                ClaimId = f.ClaimId
            }).ToListAsync();
        }

        public async Task<HiredVehicleBillResponseDto> SaveHiredVehicleBillAsync(CreateHiredVehicleBillDto dto, int userId)
        {
            var bill = dto.HiredVehicleBillId.HasValue ? await _context.HiredVehicleBills.FindAsync(dto.HiredVehicleBillId.Value) : new HiredVehicleBill();
            if (bill == null) throw new Exception("Bill not found");

            bill.BillNumber = dto.BillNumber;
            bill.BillDate = dto.BillDate;
            bill.VehicleNumber = dto.VehicleNumber;
            bill.OfficeName = dto.OfficeName;
            bill.ContractorName = dto.ContractorName;
            bill.ContractorPhone = dto.ContractorPhone;
            bill.VehicleType = dto.VehicleType;
            bill.NoOfVehicles = dto.NoOfVehicles;
            bill.HiredFrom = dto.HiredFrom;
            bill.HiredTo = dto.HiredTo;
            bill.KmCovered = dto.KmCovered;
            bill.Amount = dto.Amount;
            bill.CreatedById = userId;

            if (!dto.HiredVehicleBillId.HasValue) _context.HiredVehicleBills.Add(bill);
            await _context.SaveChangesAsync();

            return new HiredVehicleBillResponseDto { HiredVehicleBillId = bill.HiredVehicleBillId };
        }

        public async Task<bool> DeleteHiredVehicleBillAsync(int id, int userId)
        {
            var bill = await _context.HiredVehicleBills.FirstOrDefaultAsync(b => b.HiredVehicleBillId == id && b.CreatedById == userId && b.Status == BillStatus.Draft);
            if (bill == null) return false;

            _context.HiredVehicleBills.Remove(bill);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Contractual Bills

        public async Task<List<ContractualBillResponseDto>> GetContractualBillsAsync(int userId, int? claimId = null)
        {
            var query = _context.ContractualBills
                .Include(f => f.Claim)
                .Where(f => f.CreatedById == userId);

            if (claimId.HasValue)
                query = query.Where(f => f.ClaimId == claimId);
            else
                query = query.Where(f => f.Status == BillStatus.Draft);

            return await query.Select(f => new ContractualBillResponseDto
            {
                ContractualBillId = f.ContractualBillId,
                BillNumber = f.BillNumber,
                BillDate = f.BillDate,
                BillPeriodFrom = f.BillPeriodFrom,
                BillPeriodTo = f.BillPeriodTo,
                DdoCode = f.DdoCode,
                VehicleNumber = f.VehicleNumber,
                Amount = f.Amount,
                Status = f.Status,
                ClaimId = f.ClaimId
            }).ToListAsync();
        }

        public async Task<ContractualBillResponseDto> SaveContractualBillAsync(CreateContractualBillDto dto, int userId)
        {
            var bill = dto.ContractualBillId.HasValue ? await _context.ContractualBills.FindAsync(dto.ContractualBillId.Value) : new ContractualBill();
            if (bill == null) throw new Exception("Bill not found");

            bill.BillNumber = dto.BillNumber;
            bill.BillDate = dto.BillDate;
            bill.BillPeriodFrom = dto.BillPeriodFrom;
            bill.BillPeriodTo = dto.BillPeriodTo;
            bill.DdoCode = dto.DdoCode;
            bill.VehicleNumber = dto.VehicleNumber;
            bill.Amount = dto.Amount;
            bill.CreatedById = userId;

            if (!dto.ContractualBillId.HasValue) _context.ContractualBills.Add(bill);
            await _context.SaveChangesAsync();

            return new ContractualBillResponseDto { ContractualBillId = bill.ContractualBillId };
        }

        public async Task<bool> DeleteContractualBillAsync(int id, int userId)
        {
            var bill = await _context.ContractualBills.FirstOrDefaultAsync(b => b.ContractualBillId == id && b.CreatedById == userId && b.Status == BillStatus.Draft);
            if (bill == null) return false;

            _context.ContractualBills.Remove(bill);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion
    }
}
