using backend.Data;
using backend.DTOs.Billing;
using backend.Models.Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

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

        public async Task<FuelBillResponseDto?> GetFuelBillByIdAsync(int id, int userId)
        {
            var f = await _context.FuelBills
                .Include(f => f.Vehicle)
                .FirstOrDefaultAsync(f => f.FuelBillId == id && f.CreatedById == userId);

            if (f == null) return null;

            return new FuelBillResponseDto
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
                SanctionAuthorityMobileNo = f.SanctionAuthorityMobileNo,
                NocFile = f.NocFile,
                NocIssueDate = f.NocIssueDate,
                NocExpiryDate = f.NocExpiryDate,
                SanctionPermissionFile = f.SanctionPermissionFile
            };
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
            bill.BillDate = DateTime.SpecifyKind(dto.BillDate, DateTimeKind.Utc);
            bill.OdometerReading = dto.OdometerReading;
            bill.FuelQuantity = dto.FuelQuantity;
            bill.Amount = dto.Amount;
            bill.NocFile = dto.NocFile;
            bill.NocIssueDate = dto.NocIssueDate.HasValue ? DateTime.SpecifyKind(dto.NocIssueDate.Value, DateTimeKind.Utc) : null;
            bill.NocExpiryDate = dto.NocExpiryDate.HasValue ? DateTime.SpecifyKind(dto.NocExpiryDate.Value, DateTimeKind.Utc) : null;
            bill.SanctionPermissionFile = dto.SanctionPermissionFile;
            bill.SanctionAuthorityMobileNo = dto.SanctionAuthorityMobileNo;
            bill.Status = BillStatus.Draft;

            if (isNew) _context.FuelBills.Add(bill);
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
                Status = bill.Status,
                SanctionAuthorityMobileNo = bill.SanctionAuthorityMobileNo
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

        public async Task<bool> UpdateFuelBillOdometerAsync(int id, UpdateOdometerDto dto)
        {
            var bill = await _context.FuelBills.FindAsync(id);
            if (bill == null) return false;

            bill.OdometerReading = dto.OdometerReading;
            bill.BillDate = DateTime.SpecifyKind(dto.BillDate, DateTimeKind.Utc);
            
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Maintenance Bills

        public async Task<List<MaintenanceBillResponseDto>> GetMaintenanceBillsAsync(int userId, int? claimId = null)
        {
            var query = _context.MaintenanceBills
                .Include(b => b.Vehicle)
                .Where(b => b.CreatedById == userId);

            if (claimId.HasValue)
                query = query.Where(b => b.ClaimId == claimId);
            else
                query = query.Where(b => b.Status == BillStatus.Draft);

            return await query.Select(b => new MaintenanceBillResponseDto
            {
                MaintenanceBillId = b.MaintenanceBillId,
                VehicleId = b.VehicleId,
                VehicleNumber = b.Vehicle.VehicleNumber,
                BillNumber = b.BillNumber,
                BillDate = b.BillDate,
                OdometerReading = b.OdometerReading,
                Amount = b.Amount,
                MaintenanceType = b.MaintenanceType,
                Details = b.Details,
                SanctionPermissionFile = b.SanctionPermissionFile,
                Status = b.Status,
                ClaimId = b.ClaimId
            }).ToListAsync();
        }

        public async Task<MaintenanceBillResponseDto?> GetMaintenanceBillByIdAsync(int id, int userId)
        {
            return await _context.MaintenanceBills
                .Include(b => b.Vehicle)
                .Where(b => b.MaintenanceBillId == id && b.CreatedById == userId)
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
                    Details = b.Details,
                    SanctionPermissionFile = b.SanctionPermissionFile,
                    Status = b.Status,
                    ClaimId = b.ClaimId
                }).FirstOrDefaultAsync();
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
            bill.BillDate = DateTime.SpecifyKind(dto.BillDate, DateTimeKind.Utc);
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

        public async Task<bool> UpdateMaintenanceBillOdometerAsync(int id, UpdateOdometerDto dto)
        {
            var bill = await _context.MaintenanceBills.FindAsync(id);
            if (bill == null) return false;

            bill.OdometerReading = dto.OdometerReading;
            bill.BillDate = DateTime.SpecifyKind(dto.BillDate, DateTimeKind.Utc);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object?> SearchBillByIdAsync(int id)
        {
            var results = new List<object>();

            var fuelBill = await _context.FuelBills.Include(f => f.Vehicle).FirstOrDefaultAsync(f => f.FuelBillId == id);
            if (fuelBill != null)
            {
                results.Add(new { Type = 1, Bill = new FuelBillResponseDto
                {
                    FuelBillId = fuelBill.FuelBillId,
                    VehicleId = fuelBill.VehicleId,
                    VehicleNumber = fuelBill.Vehicle.VehicleNumber,
                    BillNumber = fuelBill.BillNumber,
                    BillDate = fuelBill.BillDate,
                    OdometerReading = fuelBill.OdometerReading,
                    FuelQuantity = fuelBill.FuelQuantity,
                    Amount = fuelBill.Amount,
                    Status = fuelBill.Status,
                    ClaimId = fuelBill.ClaimId
                }});
            }

            var maintBill = await _context.MaintenanceBills.Include(m => m.Vehicle).FirstOrDefaultAsync(m => m.MaintenanceBillId == id);
            if (maintBill != null)
            {
                results.Add(new { Type = 2, Bill = new MaintenanceBillResponseDto
                {
                    MaintenanceBillId = maintBill.MaintenanceBillId,
                    VehicleId = maintBill.VehicleId,
                    VehicleNumber = maintBill.Vehicle.VehicleNumber,
                    BillNumber = maintBill.BillNumber,
                    BillDate = maintBill.BillDate,
                    OdometerReading = maintBill.OdometerReading,
                    Amount = maintBill.Amount,
                    MaintenanceType = maintBill.MaintenanceType,
                    Status = maintBill.Status,
                    ClaimId = maintBill.ClaimId
                }});
            }

            return results;
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
                SanctionOrderDate = dto.SanctionOrderDate.HasValue ? DateTime.SpecifyKind(dto.SanctionOrderDate.Value, DateTimeKind.Utc) : null,
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
            else if (dto.Type == BillType.Miscellaneous)
            {
                var bills = await _context.MiscellaneousBills.Where(b => dto.BillIds.Contains(b.MiscellaneousBillId) && b.Status == BillStatus.Draft).ToListAsync();
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

        public async Task<List<BillClaimResponseDto>> GetClaimsAsync(int userId, string role, BillStatus? status = null, bool? forwardedToTreasury = null)
        {
            IQueryable<BillClaim> query = _context.BillClaims.Include(c => c.CreatedBy);

            if (role == "DDO")
            {
                query = query.Where(c => c.CreatedById == userId);
                // If a specific status is requested (e.g. status=2 from IFMS page), apply it
                if (status.HasValue)
                    query = query.Where(c => c.Status == status.Value);
            }
            else if (role == "ADMN" || role == "NDOF")
            {
                if (status.HasValue)
                    query = query.Where(c => c.Status == status.Value);
                else
                    query = query.Where(c => c.Status == BillStatus.Pending);
            }

            if (forwardedToTreasury.HasValue)
            {
                query = query.Where(c => c.ForwardedToTreasury == forwardedToTreasury.Value);
            }

            return await query.Select(c => new BillClaimResponseDto
            {
                BillClaimId = c.BillClaimId,
                ClaimNumber = c.ClaimNumber,
                TotalAmount = c.TotalAmount,
                Status = c.Status,
                Type = c.Type,
                CreatedBy = $"{c.CreatedBy.FirstName} {c.CreatedBy.LastName}",
                CreatedAt = c.CreatedAt,
                Comments = c.Comments,
                ForwardedToTreasury = c.ForwardedToTreasury
            }).ToListAsync();
        }

        public async Task<List<BillClaimResponseDto>> GetPendingIntegrationClaimsAsync(int userId, string role)
        {
            IQueryable<BillClaim> query = _context.BillClaims.Include(c => c.CreatedBy);

            if (role == "DDO")
            {
                query = query.Where(c => c.CreatedById == userId);
            }
            
            query = query.Where(c => (c.Status == BillStatus.Verified || c.Status == BillStatus.Discarded) && c.ForwardedToTreasury == false);

            return await query.Select(c => new BillClaimResponseDto
            {
                BillClaimId = c.BillClaimId,
                ClaimNumber = c.ClaimNumber,
                TotalAmount = c.TotalAmount,
                Status = c.Status,
                Type = c.Type,
                CreatedBy = c.CreatedBy != null ? $"{c.CreatedBy.FirstName} {c.CreatedBy.LastName}" : null,
                CreatedAt = c.CreatedAt,
                Comments = c.Comments,
                ForwardedToTreasury = c.ForwardedToTreasury
            }).OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<BillClaimDetailDto?> GetClaimByIdAsync(int id)
        {
            var claim = await _context.BillClaims
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(c => c.BillClaimId == id);

            return await GetClaimDetailInternalAsync(claim);
        }

        public async Task<BillClaimDetailDto?> GetClaimByNumberAsync(string claimNumber)
        {
            var claim = await _context.BillClaims
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);

            return await GetClaimDetailInternalAsync(claim);
        }

        private async Task<BillClaimDetailDto?> GetClaimDetailInternalAsync(BillClaim? claim)
        {
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
            else if (claim.Type == BillType.Miscellaneous)
            {
                dto.MiscellaneousBills = await _context.MiscellaneousBills
                    .Include(b => b.InventoryItem)
                    .Where(b => b.ClaimId == claim.BillClaimId)
                    .Select(b => new MiscellaneousBillResponseDto
                    {
                        MiscellaneousBillId = b.MiscellaneousBillId,
                        BillNumber = b.BillNumber,
                        BillDate = b.BillDate,
                        InventoryMasterId = b.InventoryItemId,
                        InventoryName = b.InventoryItem.Name,
                        ModelNumber = b.ModelNumber,
                        Quantity = b.Quantity,
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

            var miscBills = await _context.MiscellaneousBills.Where(b => b.ClaimId == claimId).ToListAsync();
            miscBills.ForEach(b => b.Status = BillStatus.Verified);

            await _context.SaveChangesAsync();

            // Sync to legacy IFMS integration tables if forwarded to treasury
            await SyncClaimToLegacyIfmsAsync(claimId);

            return true;
        }

        private async Task SyncClaimToLegacyIfmsAsync(int claimId)
        {
            var claim = await _context.BillClaims.Include(c => c.CreatedBy).FirstOrDefaultAsync(c => c.BillClaimId == claimId);
            if (claim == null || !claim.ForwardedToTreasury) return;

            var conn = _context.Database.GetDbConnection();
            bool wasClosed = conn.State == ConnectionState.Closed;
            if (wasClosed) await conn.OpenAsync();

            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    // 1. Get next FuelMaintenanceIFMSId
                    cmd.CommandText = "SELECT COALESCE(MAX(\"FuelMaintenanceIFMSId\"), 0) + 1 FROM \"tblFuelMaintenanceDetails_IFMS\"";
                    int nextIfmsId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                    // 2. Insert into tblFuelMaintenanceDetails_IFMS
                    cmd.CommandText = @"
                        INSERT INTO ""tblFuelMaintenanceDetails_IFMS"" (
                            ""FuelMaintenanceIFMSId"", ""FuelMaintenance"", ""ActionDate"", ""Amount"", 
                            ""SubVoucherNo"", ""SubVoucherDesc"", ""SanctionOrderNo"", ""SanctionOrderDate"", 
                            ""SanctionAuthority"", ""FwdToTreasury"", ""Status"", ""ClaimNo"", 
                            ""DDOCode"", ""IFMSStatus"", ""incomeTaxAmount"", ""IsGrantInAidBill""
                        ) VALUES (
                            @ifmsId, @type, @actionDate, @amount, 
                            @subVoucherNo, @subVoucherDesc, @sanctionOrderNo, @sanctionOrderDate, 
                            @sanctionAuthority, true, 'Pending', @claimNo, 
                            @ddoCode, 2, @tax, false
                        )";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new NpgsqlParameter("ifmsId", nextIfmsId));
                    cmd.Parameters.Add(new NpgsqlParameter("type", (int)claim.Type));
                    cmd.Parameters.Add(new NpgsqlParameter("actionDate", claim.CreatedAt));
                    cmd.Parameters.Add(new NpgsqlParameter("amount", (int)claim.TotalAmount));
                    cmd.Parameters.Add(new NpgsqlParameter("subVoucherNo", (object?)claim.SubVoucherNo ?? DBNull.Value));
                    cmd.Parameters.Add(new NpgsqlParameter("subVoucherDesc", (object?)claim.SubVoucherDescription ?? DBNull.Value));
                    cmd.Parameters.Add(new NpgsqlParameter("sanctionOrderNo", (object?)claim.SanctionOrderNo ?? DBNull.Value));
                    cmd.Parameters.Add(new NpgsqlParameter("sanctionOrderDate", (object?)claim.SanctionOrderDate ?? DBNull.Value));
                    cmd.Parameters.Add(new NpgsqlParameter("sanctionAuthority", (object?)claim.SanctionAuthority ?? DBNull.Value));
                    cmd.Parameters.Add(new NpgsqlParameter("claimNo", claim.ClaimNumber));
                    cmd.Parameters.Add(new NpgsqlParameter("ddoCode", claim.CreatedBy.DDOCode));
                    cmd.Parameters.Add(new NpgsqlParameter("tax", (int)claim.Tax));

                    await cmd.ExecuteNonQueryAsync();

                    // 3. Insert child bills into legacy tables
                    if (claim.Type == BillType.Fuel || claim.Type == BillType.Maintenance)
                    {
                        var fuelBills = await _context.FuelBills.Where(b => b.ClaimId == claimId).ToListAsync();
                        foreach (var b in fuelBills)
                        {
                            cmd.CommandText = "SELECT COALESCE(MAX(\"FuelMaintenanceId\"), 0) + 1 FROM \"tblFuelMaintenanceDetails\"";
                            int nextFuelId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                            cmd.CommandText = @"
                                INSERT INTO ""tblFuelMaintenanceDetails"" (
                                    ""FuelMaintenanceId"", ""FuelMaintenanceIFMSId"", ""VehicleInfoId"", 
                                    ""Amount"", ""OdometerReading"", ""Status"", ""PDate"", ""BillNumber"", ""FuelMaintenance""
                                ) VALUES (
                                    @fuelId, @ifmsId, @vehicleId, 
                                    @amount, @odometer, 200, @pDate, @billNo, @claimType
                                )";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new NpgsqlParameter("fuelId", nextFuelId));
                            cmd.Parameters.Add(new NpgsqlParameter("ifmsId", nextIfmsId));
                            cmd.Parameters.Add(new NpgsqlParameter("vehicleId", b.VehicleId));
                            cmd.Parameters.Add(new NpgsqlParameter("amount", (int)b.Amount));
                            cmd.Parameters.Add(new NpgsqlParameter("odometer", b.OdometerReading));
                            cmd.Parameters.Add(new NpgsqlParameter("pDate", b.BillDate));
                            cmd.Parameters.Add(new NpgsqlParameter("billNo", b.BillNumber));
                            cmd.Parameters.Add(new NpgsqlParameter("claimType", (int)claim.Type));
                            await cmd.ExecuteNonQueryAsync();
                        }

                        var maintBills = await _context.MaintenanceBills.Where(b => b.ClaimId == claimId).ToListAsync();
                        foreach (var b in maintBills)
                        {
                            cmd.CommandText = "SELECT COALESCE(MAX(\"FuelMaintenanceId\"), 0) + 1 FROM \"tblFuelMaintenanceDetails\"";
                            int nextFuelId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                            cmd.CommandText = @"
                                INSERT INTO ""tblFuelMaintenanceDetails"" (
                                    ""FuelMaintenanceId"", ""FuelMaintenanceIFMSId"", ""VehicleInfoId"", 
                                    ""Amount"", ""OdometerReading"", ""Status"", ""PDate"", ""BillNumber"", ""FuelMaintenance""
                                ) VALUES (
                                    @fuelId, @ifmsId, @vehicleId, 
                                    @amount, @odometer, 200, @pDate, @billNo, @claimType
                                )";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new NpgsqlParameter("fuelId", nextFuelId));
                            cmd.Parameters.Add(new NpgsqlParameter("ifmsId", nextIfmsId));
                            cmd.Parameters.Add(new NpgsqlParameter("vehicleId", b.VehicleId));
                            cmd.Parameters.Add(new NpgsqlParameter("amount", (int)b.Amount));
                            cmd.Parameters.Add(new NpgsqlParameter("odometer", b.OdometerReading));
                            cmd.Parameters.Add(new NpgsqlParameter("pDate", b.BillDate));
                            cmd.Parameters.Add(new NpgsqlParameter("billNo", b.BillNumber));
                            cmd.Parameters.Add(new NpgsqlParameter("claimType", (int)claim.Type));
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    else if (claim.Type == BillType.Hired)
                    {
                        var hiredBills = await _context.HiredVehicleBills.Where(b => b.ClaimId == claimId).ToListAsync();
                        foreach (var b in hiredBills)
                        {
                            cmd.CommandText = "SELECT COALESCE(MAX(\"HireVehicleDetailsId\"), 0) + 1 FROM \"tblHireVehicleDetails\"";
                            int nextHiredId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                            cmd.CommandText = @"
                                INSERT INTO ""tblHireVehicleDetails"" (
                                    ""HireVehicleDetailsId"", ""FuelMaintenanceIFMSId"", ""VehicleNumber"", 
                                    ""BillAmount"", ""BillNumber"", ""BillDate"", ""Status"", ""OfficeId"", ""NoofVehicles"", ""KMCovered""
                                ) VALUES (
                                    @hiredId, @ifmsId, @vehicleNo, 
                                    @amount, @billNo, @pDate, 200, 1, @noofVehicles, @kmCovered
                                )";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new NpgsqlParameter("hiredId", nextHiredId));
                            cmd.Parameters.Add(new NpgsqlParameter("ifmsId", nextIfmsId));
                            cmd.Parameters.Add(new NpgsqlParameter("vehicleNo", b.VehicleNumber));
                            cmd.Parameters.Add(new NpgsqlParameter("amount", (int)b.Amount));
                            cmd.Parameters.Add(new NpgsqlParameter("billNo", b.BillNumber));
                            cmd.Parameters.Add(new NpgsqlParameter("pDate", b.BillDate));
                            cmd.Parameters.Add(new NpgsqlParameter("noofVehicles", b.NoOfVehicles));
                            cmd.Parameters.Add(new NpgsqlParameter("kmCovered", (double)b.KmCovered));
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    else if (claim.Type == BillType.Miscellaneous)
                    {
                        var miscBills = await _context.MiscellaneousBills.Where(b => b.ClaimId == claimId).ToListAsync();
                        foreach (var b in miscBills)
                        {
                            cmd.CommandText = "SELECT COALESCE(MAX(\"MTStoreId\"), 0) + 1 FROM \"tblMTStore\"";
                            int nextMiscId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                            cmd.CommandText = @"
                                INSERT INTO ""tblMTStore"" (
                                    ""MTStoreId"", ""FuelMaintenanceIFMSId"", ""BillNumber"", 
                                    ""BillDate"", ""BillAmount"", ""Status"", ""Quantity""
                                ) VALUES (
                                    @miscId, @ifmsId, @billNo, 
                                    @pDate, @amount, 200, @quantity
                                )";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new NpgsqlParameter("miscId", nextMiscId));
                            cmd.Parameters.Add(new NpgsqlParameter("ifmsId", nextIfmsId));
                            cmd.Parameters.Add(new NpgsqlParameter("billNo", b.BillNumber));
                            cmd.Parameters.Add(new NpgsqlParameter("pDate", b.BillDate));
                            cmd.Parameters.Add(new NpgsqlParameter("amount", (int)b.Amount));
                            cmd.Parameters.Add(new NpgsqlParameter("quantity", (double)b.Quantity));
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    else if (claim.Type == BillType.Contractual)
                    {
                        var contractBills = await _context.ContractualBills.Where(b => b.ClaimId == claimId).ToListAsync();
                        foreach (var b in contractBills)
                        {
                            cmd.CommandText = "SELECT COALESCE(MAX(\"ContractualClaimId\"), 0) + 1 FROM \"tblContractualClaim\"";
                            int nextContractId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                            cmd.CommandText = @"
                                INSERT INTO ""tblContractualClaim"" (
                                    ""ContractualClaimId"", ""FuelMaintenanceIFMSId"", ""BillNumber"", 
                                    ""BillDate"", ""BillAmount"", ""Status"", ""VehicleNumber""
                                ) VALUES (
                                    @contractId, @ifmsId, @billNo, 
                                    @pDate, @amount, 200, @vehicleNo
                                )";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new NpgsqlParameter("contractId", nextContractId));
                            cmd.Parameters.Add(new NpgsqlParameter("ifmsId", nextIfmsId));
                            cmd.Parameters.Add(new NpgsqlParameter("billNo", b.BillNumber));
                            cmd.Parameters.Add(new NpgsqlParameter("pDate", b.BillDate));
                            cmd.Parameters.Add(new NpgsqlParameter("amount", (int)b.Amount));
                            cmd.Parameters.Add(new NpgsqlParameter("vehicleNo", b.VehicleNumber));
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            finally
            {
                if (wasClosed) await conn.CloseAsync();
            }
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

            var miscBills = await _context.MiscellaneousBills.Where(b => b.ClaimId == claimId).ToListAsync();
            foreach(var b in miscBills) { b.Status = BillStatus.Draft; b.ClaimId = null; }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DiscardClaimAsync(int claimId, int userId, string? comments)
        {
            var claim = await _context.BillClaims.FindAsync(claimId);
            // Can only discard bills that are verified (Ready for IFMS) or pending
            if (claim == null || (claim.Status != BillStatus.Verified && claim.Status != BillStatus.Pending)) return false;

            claim.Status = BillStatus.Discarded;
            claim.VerifiedById = userId;
            claim.VerifiedAt = DateTime.UtcNow;
            claim.Comments = comments;

            // Also mark individual bills as discarded
            var fuelBills = await _context.FuelBills.Where(b => b.ClaimId == claimId).ToListAsync();
            fuelBills.ForEach(b => b.Status = BillStatus.Discarded);

            var maintBills = await _context.MaintenanceBills.Where(b => b.ClaimId == claimId).ToListAsync();
            maintBills.ForEach(b => b.Status = BillStatus.Discarded);

            var hiredBills = await _context.HiredVehicleBills.Where(b => b.ClaimId == claimId).ToListAsync();
            hiredBills.ForEach(b => b.Status = BillStatus.Discarded);

            var contractBills = await _context.ContractualBills.Where(b => b.ClaimId == claimId).ToListAsync();
            contractBills.ForEach(b => b.Status = BillStatus.Discarded);

            var miscBills = await _context.MiscellaneousBills.Where(b => b.ClaimId == claimId).ToListAsync();
            miscBills.ForEach(b => b.Status = BillStatus.Discarded);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreClaimAsync(int claimId, int userId, string? comments)
        {
            var claim = await _context.BillClaims.FindAsync(claimId);
            if (claim == null || claim.Status != BillStatus.Discarded) return false;

            // Restore back to Verified state (Ready for IFMS)
            claim.Status = BillStatus.Verified;
            claim.VerifiedById = userId;
            claim.VerifiedAt = DateTime.UtcNow;
            claim.Comments = comments;

            var fuelBills = await _context.FuelBills.Where(b => b.ClaimId == claimId).ToListAsync();
            fuelBills.ForEach(b => b.Status = BillStatus.Verified);

            var maintBills = await _context.MaintenanceBills.Where(b => b.ClaimId == claimId).ToListAsync();
            maintBills.ForEach(b => b.Status = BillStatus.Verified);

            var hiredBills = await _context.HiredVehicleBills.Where(b => b.ClaimId == claimId).ToListAsync();
            hiredBills.ForEach(b => b.Status = BillStatus.Verified);

            var contractBills = await _context.ContractualBills.Where(b => b.ClaimId == claimId).ToListAsync();
            contractBills.ForEach(b => b.Status = BillStatus.Verified);

            var miscBills = await _context.MiscellaneousBills.Where(b => b.ClaimId == claimId).ToListAsync();
            miscBills.ForEach(b => b.Status = BillStatus.Verified);

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
            bool isNew = !dto.HiredVehicleBillId.HasValue || dto.HiredVehicleBillId.Value <= 0;
            HiredVehicleBill bill;
            
            if (!isNew)
            {
                bill = await _context.HiredVehicleBills.FindAsync(dto.HiredVehicleBillId!.Value) ?? throw new Exception("Bill not found");
                if (bill.CreatedById != userId) throw new UnauthorizedAccessException();
                if (bill.Status != BillStatus.Draft) throw new Exception("Only draft bills can be modified");
            }
            else
            {
                bill = new HiredVehicleBill { CreatedById = userId, CreatedAt = DateTime.UtcNow };
            }

            bill.BillNumber = dto.BillNumber;
            bill.BillDate = DateTime.SpecifyKind(dto.BillDate, DateTimeKind.Utc);
            bill.VehicleNumber = dto.VehicleNumber;
            bill.OfficeName = dto.OfficeName;
            bill.ContractorName = dto.ContractorName;
            bill.ContractorPhone = dto.ContractorPhone;
            bill.VehicleType = dto.VehicleType;
            bill.NoOfVehicles = dto.NoOfVehicles;
            bill.HiredFrom = DateTime.SpecifyKind(dto.HiredFrom, DateTimeKind.Utc);
            bill.HiredTo = DateTime.SpecifyKind(dto.HiredTo, DateTimeKind.Utc);
            bill.KmCovered = dto.KmCovered;
            bill.Amount = dto.Amount;
            bill.CreatedById = userId;

            bill.Status = BillStatus.Draft;
            if (isNew) _context.HiredVehicleBills.Add(bill);
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
            bool isNew = !dto.ContractualBillId.HasValue || dto.ContractualBillId.Value <= 0;
            ContractualBill bill;

            if (!isNew)
            {
                bill = await _context.ContractualBills.FindAsync(dto.ContractualBillId!.Value) ?? throw new Exception("Bill not found");
                if (bill.CreatedById != userId) throw new UnauthorizedAccessException();
                if (bill.Status != BillStatus.Draft) throw new Exception("Only draft bills can be modified");
            }
            else
            {
                bill = new ContractualBill { CreatedById = userId, CreatedAt = DateTime.UtcNow };
            }

            bill.BillNumber = dto.BillNumber;
            bill.BillDate = DateTime.SpecifyKind(dto.BillDate, DateTimeKind.Utc);
            bill.BillPeriodFrom = DateTime.SpecifyKind(dto.BillPeriodFrom, DateTimeKind.Utc);
            bill.BillPeriodTo = DateTime.SpecifyKind(dto.BillPeriodTo, DateTimeKind.Utc);
            bill.DdoCode = dto.DdoCode;
            bill.VehicleNumber = dto.VehicleNumber;
            bill.Amount = dto.Amount;
            bill.CreatedById = userId;

            bill.Status = BillStatus.Draft;
            if (isNew) _context.ContractualBills.Add(bill);
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

        #region Miscellaneous Bills

        public async Task<List<MiscellaneousBillResponseDto>> GetMiscellaneousBillsAsync(int userId, int? claimId = null)
        {
            var query = _context.MiscellaneousBills
                .Include(f => f.InventoryItem)
                .Include(f => f.Claim)
                .Where(f => f.CreatedById == userId);

            if (claimId.HasValue)
                query = query.Where(f => f.ClaimId == claimId);
            else
                query = query.Where(f => f.Status == BillStatus.Draft);

            return await query.Select(f => new MiscellaneousBillResponseDto
            {
                MiscellaneousBillId = f.MiscellaneousBillId,
                BillNumber = f.BillNumber,
                BillDate = f.BillDate,
                InventoryMasterId = f.InventoryItemId,
                InventoryName = f.InventoryItem.Name,
                ModelNumber = f.ModelNumber,
                Quantity = f.Quantity,
                Amount = f.Amount,
                Status = f.Status,
                ClaimId = f.ClaimId
            }).ToListAsync();
        }

        public async Task<MiscellaneousBillResponseDto> SaveMiscellaneousBillAsync(CreateMiscellaneousBillDto dto, int userId)
        {
            bool isNew = !dto.MiscellaneousBillId.HasValue || dto.MiscellaneousBillId.Value <= 0;
            MiscellaneousBill bill;

            if (!isNew)
            {
                bill = await _context.MiscellaneousBills.FindAsync(dto.MiscellaneousBillId!.Value) ?? throw new Exception("Bill not found");
                if (bill.CreatedById != userId) throw new UnauthorizedAccessException();
                if (bill.Status != BillStatus.Draft) throw new Exception("Only draft bills can be modified");
            }
            else
            {
                bill = new MiscellaneousBill { CreatedById = userId, CreatedAt = DateTime.UtcNow };
            }

            bill.BillNumber = dto.BillNumber;
            bill.BillDate = DateTime.SpecifyKind(dto.BillDate, DateTimeKind.Utc);
            bill.InventoryItemId = dto.InventoryMasterId;
            bill.ModelNumber = dto.ModelNumber;
            bill.Quantity = dto.Quantity;
            bill.Amount = dto.Amount;
            bill.Status = BillStatus.Draft;

            if (isNew) _context.MiscellaneousBills.Add(bill);
            await _context.SaveChangesAsync();

            return new MiscellaneousBillResponseDto { MiscellaneousBillId = bill.MiscellaneousBillId };
        }

        public async Task<bool> DeleteMiscellaneousBillAsync(int id, int userId)
        {
            var bill = await _context.MiscellaneousBills.FirstOrDefaultAsync(b => b.MiscellaneousBillId == id && b.CreatedById == userId && b.Status == BillStatus.Draft);
            if (bill == null) return false;

            _context.MiscellaneousBills.Remove(bill);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Personal Usage
        public async Task<bool> InsertPersonalUseDetailsAsync(PersonalUsagePayloadDto dto)
        {
            var plan = new PersonalUsagePlan
            {
                RecordId = dto.record_id,
                ItemId = dto.item_id,
                PlanId = dto.plan_id,
                VehicleInfoId = dto.vehicle_info_id,
                VehicleNo = dto.vehicle_no,
                CreatedAt = DateTime.UtcNow,
                UsageLogs = new List<PersonalUsageLog>()
            };

            if (!string.IsNullOrEmpty(dto.personal_use_details))
            {
                try
                {
                    var logs = System.Text.Json.JsonSerializer.Deserialize<List<PersonalUsageLogDto>>(dto.personal_use_details);
                    if (logs != null)
                    {
                        foreach(var log in logs)
                        {
                            plan.UsageLogs.Add(new PersonalUsageLog
                            {
                                OfficerId = log.OfficerId,
                                DateOfUse = DateTime.Parse(log.DateOfUse).ToUniversalTime(),
                                OdometerFrom = log.OMFrom,
                                OdometerTo = log.OMTo
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If JSON is malformed, we still save the plan but without logs, or we can throw.
                    throw new Exception("Invalid personal use details JSON format.", ex);
                }
            }

            _context.PersonalUsagePlans.Add(plan);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
