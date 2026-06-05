using backend.Data;
using backend.DTOs;
using backend.DTOs.Masters;
using backend.Models.Core;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public VehicleService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        private DateTime? ToUtc(DateTime? date)
        {
            if (!date.HasValue) return null;
            return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
        }

        public async Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var ddoCode = user?.DDOCode ?? string.Empty;

            var vehicle = new VehicleInfo
            {
                VehicleNumber = dto.VehicleNumber,
                EngineChasisNumber = dto.ChassisNumber ?? string.Empty,
                ManufacturerId = dto.ManufacturerId,
                ModelId = dto.ModelId ?? 0,
                ManufactureYear = int.TryParse(dto.ManufactureYear, out int year) ? year : 0,
                VehicleTypeId = dto.VehicleTypeId,
                SeatingCapacity = dto.SeatingCapacity?.ToString() ?? string.Empty,
                FuelUsed = dto.FuelUsed,
                OfficeId = dto.OfficeId,
                DeptId = null, // Logic for DeptId can be added if needed from Office
                DesignationId = dto.DesignationId,
                OfficerId = dto.OfficerId,
                HRMSCode = dto.HRMSCode ?? string.Empty,
                ProjectId = dto.ProjectId,
                OfficerName = dto.OfficerName ?? string.Empty,
                AllocationType = dto.VehicleAllocationType ?? string.Empty,
                CurrentStatus = dto.CurrentStatus ?? string.Empty,
                VehicleCost = dto.VehicleCost,
                VehiclePurchaseDate = ToUtc(dto.PurchaseDate),
                FitnessUpto = ToUtc(dto.FitnessUpto),
                DriverType = dto.DriverType ?? string.Empty,
                DriverName = dto.DriverName ?? string.Empty,
                DriverContactNo = dto.DriverContactNo ?? string.Empty,
                ContractorName = dto.ContractorName ?? string.Empty,
                ContractorContactNo = dto.ContractorContactNo ?? string.Empty,
                DDOId = ddoCode,
                CreatedBy = userId.ToString(),
                UpdatedDate = null,
                IsActive = true,
                verificationstatus = 0, // Unverified
                Ishaveyoupurchasednewvehicle = dto.Ishaveyoupurchasednewvehicle,
                IsTyreOriginal = dto.IsTyreOriginal,
                LastTyreChangedDate = ToUtc(dto.TyreChangedDate),
                LastTyreChangedKM = dto.TyreChangedMeterReading,
                KM_30062017 = dto.KM30062017,
                FuelConsumptionCost = dto.FuelConsumptionCost,
                FuelConsumptionLitres = dto.FuelConsumptionLitres,
                LastThreeYearsMaintenanceCost = dto.Last3YearsMaintenanceCost,
                VehiclePhotoPath = dto.VehiclePhoto != null ? await _fileService.SaveFileAsync(dto.VehiclePhoto, "vehicles/photos") ?? "" : "",
                RegistrationCertificatePath = dto.RegistrationCertificate != null ? await _fileService.SaveFileAsync(dto.RegistrationCertificate, "vehicles/certs") ?? "" : "",
                FdApprovalPath = dto.FdApprovalFile != null ? await _fileService.SaveFileAsync(dto.FdApprovalFile, "vehicles/approvals") ?? "" : "",
                FleetStrengthLetterPath = dto.FleetStrengthLetterFile != null ? await _fileService.SaveFileAsync(dto.FleetStrengthLetterFile, "vehicles/fleet") ?? "" : ""
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            // Reload to get navigation properties
            return await GetVehicleByIdAsync(vehicle.VehicleInfoId) ?? MapToResponseDto(vehicle);
        }

        public async Task<List<VehicleResponseDto>> GetAllVehiclesAsync(int? status = null)
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Manufacturer)
                .Include(v => v.Model)
                .Include(v => v.VehicleType)
                .Include(v => v.Office)
                    .ThenInclude(o => o.District)
                .Include(v => v.Office)
                    .ThenInclude(o => o.Tehsil)
                .Include(v => v.Designation)
                .Include(v => v.Department)
                .Where(v => v.IsActive && (!status.HasValue || v.verificationstatus == status.Value))
                .ToListAsync();

            return vehicles.Select(MapToResponseDto).ToList();
        }

        public async Task<VehicleResponseDto?> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Manufacturer)
                .Include(v => v.Model)
                .Include(v => v.VehicleType)
                .Include(v => v.Office)
                    .ThenInclude(o => o.District)
                .Include(v => v.Office)
                    .ThenInclude(o => o.Tehsil)
                .Include(v => v.Designation)
                .Include(v => v.Department)
                .Include(v => v.Project)
                .Include(v => v.RequisitionDepartment)
                .Include(v => v.RequisitionOffice)
                .FirstOrDefaultAsync(v => v.VehicleInfoId == id);

            return vehicle != null ? MapToResponseDto(vehicle) : null;
        }

        public async Task<VehicleResponseDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return null;

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.EngineChasisNumber = dto.ChassisNumber ?? string.Empty;
            vehicle.ManufacturerId = dto.ManufacturerId;
            vehicle.ModelId = dto.ModelId ?? 0;
            vehicle.ManufactureYear = int.TryParse(dto.ManufactureYear, out int year) ? year : 0;
            vehicle.VehicleTypeId = dto.VehicleTypeId;
            vehicle.SeatingCapacity = dto.SeatingCapacity?.ToString() ?? string.Empty;
            vehicle.FuelUsed = dto.FuelUsed;
            vehicle.OfficeId = dto.OfficeId;
            vehicle.DesignationId = dto.DesignationId;
            vehicle.OfficerId = dto.OfficerId;
            vehicle.HRMSCode = dto.HRMSCode ?? string.Empty;
            vehicle.ProjectId = dto.ProjectId;
            vehicle.OfficerName = dto.OfficerName ?? string.Empty;
            vehicle.AllocationType = dto.VehicleAllocationType ?? string.Empty;
            vehicle.CurrentStatus = dto.CurrentStatus ?? string.Empty;
            vehicle.VehicleCost = dto.VehicleCost;
            vehicle.VehiclePurchaseDate = ToUtc(dto.PurchaseDate);
            vehicle.FitnessUpto = ToUtc(dto.FitnessUpto);
            vehicle.DriverType = dto.DriverType ?? string.Empty;
            vehicle.DriverName = dto.DriverName ?? string.Empty;
            vehicle.DriverContactNo = dto.DriverContactNo ?? string.Empty;
            vehicle.ContractorName = dto.ContractorName ?? string.Empty;
            vehicle.ContractorContactNo = dto.ContractorContactNo ?? string.Empty;
            vehicle.Ishaveyoupurchasednewvehicle = dto.Ishaveyoupurchasednewvehicle;
            vehicle.IsTyreOriginal = dto.IsTyreOriginal;
            vehicle.LastTyreChangedDate = ToUtc(dto.TyreChangedDate);
            vehicle.LastTyreChangedKM = dto.TyreChangedMeterReading;
            vehicle.KM_30062017 = dto.KM30062017;
            vehicle.FuelConsumptionCost = dto.FuelConsumptionCost;
            vehicle.FuelConsumptionLitres = dto.FuelConsumptionLitres;
            vehicle.LastThreeYearsMaintenanceCost = dto.Last3YearsMaintenanceCost;
            vehicle.UpdatedDate = DateTime.UtcNow;

            if (dto.VehiclePhoto != null)
                vehicle.VehiclePhotoPath = await _fileService.SaveFileAsync(dto.VehiclePhoto, "vehicles/photos") ?? "";
            if (dto.RegistrationCertificate != null)
                vehicle.RegistrationCertificatePath = await _fileService.SaveFileAsync(dto.RegistrationCertificate, "vehicles/certs") ?? "";
            if (dto.FdApprovalFile != null)
                vehicle.FdApprovalPath = await _fileService.SaveFileAsync(dto.FdApprovalFile, "vehicles/approvals") ?? "";
            if (dto.FleetStrengthLetterFile != null)
                vehicle.FleetStrengthLetterPath = await _fileService.SaveFileAsync(dto.FleetStrengthLetterFile, "vehicles/fleet") ?? "";

            await _context.SaveChangesAsync();
            return await GetVehicleByIdAsync(vehicle.VehicleInfoId);
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return false;

            vehicle.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyVehicleAsync(int id, string verifierId, string comments, int status)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return false;

            vehicle.verificationstatus = status;
            vehicle.VerifierId = verifierId;
            vehicle.Comments = comments;
            vehicle.VerificationDate = DateTime.UtcNow;

            // If DDOId is empty (legacy issue), attempt to populate it from the creator's DDO Code
            if (string.IsNullOrEmpty(vehicle.DDOId) && int.TryParse(vehicle.CreatedBy, out int creatorId))
            {
                var creator = await _context.Users.FindAsync(creatorId);
                if (creator != null && !string.IsNullOrEmpty(creator.DDOCode))
                {
                    vehicle.DDOId = creator.DDOCode;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferVehicleAsync(TransferVehicleDto dto, int userId)
        {
            var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
            if (vehicle == null) return false;

            var targetOffice = await _context.Offices.FindAsync(dto.ToOfficeId);
            if (targetOffice == null) return false;

            int previousOfficeId = vehicle.OfficeId;
            string transferOrderPath = "";

            if (dto.TransferOrderFile != null)
            {
                transferOrderPath = await _fileService.SaveFileAsync(dto.TransferOrderFile, "vehicles/transfers") ?? "";
            }

            // 1. Create Transfer History Record
            var transfer = new VehicleTransfer
            {
                VehicleId = vehicle.VehicleInfoId,
                FromOfficeId = previousOfficeId,
                ToOfficeId = targetOffice.OfficeId,
                TransferDate = DateTime.SpecifyKind(dto.TransferDate, DateTimeKind.Utc),
                TransferOrderNumber = dto.TransferOrderNumber,
                TransferOrderPath = transferOrderPath,
                Remarks = dto.Remarks,
                FromAllocationType = vehicle.AllocationType,
                FromOfficerName = vehicle.OfficerName,
                FromDesignationName = vehicle.Designation?.DesignationName ?? string.Empty,
                FromDdoCode = vehicle.DDOId,
                ToDdoCode = string.Empty, // Fixed: targetOffice does not have DDOCode
                CreatedBy = userId.ToString()
            };

            // 2. Update Vehicle Current Office & Allocation Details (Legacy Parity)
            vehicle.OfficeId = dto.ToOfficeId;
            vehicle.DeptId = dto.ToDeptId;
            vehicle.AllocationType = dto.ToAllocationType ?? string.Empty;
            vehicle.DesignationId = dto.ToDesignationId;
            vehicle.OfficerId = dto.ToOfficerId;
            vehicle.HRMSCode = dto.ToHRMSCode ?? string.Empty;
            vehicle.OfficerName = dto.ToOfficerName ?? string.Empty;
            vehicle.DDOId = string.Empty; // Reset DDO code until next verification/assignment
            vehicle.UpdatedDate = DateTime.UtcNow;

            _context.VehicleTransfers.Add(transfer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RegisterReplacementVehicleAsync(RegisterReplacementVehicleDto dto, int userId)
        {
            var searchRegNo = dto.CondemnedVehicleRegNo?.Trim().ToUpper();
            var condemnedVehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleNumber.ToUpper() == searchRegNo);
            if (condemnedVehicle == null) return false;

            // Wait, we need to create the replacement vehicle first or link it. 
            // In our system, the replacement vehicle is just registered via a string NewVehicleRegNo?
            // Yes, let's just create a VehicleCondemnation record with the FD approval doc.
            
            string fdDocPath = "";
            if (dto.FdApprovalDoc != null)
            {
                fdDocPath = await _fileService.SaveFileAsync(dto.FdApprovalDoc, "vehicles/condemnation/fdapproval") ?? "";
            }

            var condemnation = new VehicleCondemnation
            {
                VehicleId = condemnedVehicle.VehicleInfoId,
                ReplacementVehicleRegNo = dto.NewVehicleRegNo ?? "",
                ReplacementVehicleChassisNo = dto.NewVehicleChassisNo ?? "",
                FdApprovalDocPath = fdDocPath ?? "",
                IsReplacementRegistered = true,
                CondemnationDate = DateTime.UtcNow,
                CondemnationOrderNumber = "N/A",
                Reason = "Register Replacement Vehicle",
                AuctionStatus = "Pending",
                CondemnationOrderPath = "",
                GRNNumber = "",
                GrnDocumentPath = "",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.VehicleCondemnations.Add(condemnation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkForCondemnedAsync(MarkForCondemnedDto dto, int userId)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleNumber == dto.VehicleNumber);
            if (vehicle == null) return false;

            // Update Vehicle Status
            vehicle.CurrentStatus = "Marked for Condemned by FD";
            vehicle.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectCondemnationAsync(string vehicleNumber, string reason, int userId)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleNumber == vehicleNumber);
            if (vehicle == null) return false;

            // Find the pending condemnation record
            var condemnation = await _context.VehicleCondemnations
                .Where(c => c.VehicleId == vehicle.VehicleInfoId && c.IsReplacementRegistered)
                .OrderByDescending(c => c.CreatedDate)
                .FirstOrDefaultAsync();

            if (condemnation != null)
            {
                // Delete the pending request so DDO can try again
                _context.VehicleCondemnations.Remove(condemnation);
                
                // Optionally log the reason or notify DDO (left out of scope for now)
                
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> AddVehicleGrnNumberAndDetailsAsync(VehicleGrnDetailsDto dto, int userId)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleNumber == dto.OldVehicleNumber);
            if (vehicle == null) return false;

            // Only allow if marked for condemned or already condemned (for multiple GRN deposits)
            if (vehicle.CurrentStatus != "Marked for Condemned by FD" && vehicle.CurrentStatus != "CONDEMNED") return false;

            string grnDocPath = "";
            if (dto.IsOldGrn && dto.GrnDoc != null)
            {
                grnDocPath = await _fileService.SaveFileAsync(dto.GrnDoc, "vehicles/condemnation/grn") ?? "";
            }

            var condemnation = new VehicleCondemnation
            {
                VehicleId = vehicle.VehicleInfoId,
                CondemnationDate = DateTime.UtcNow,
                CondemnationOrderNumber = "N/A", // From legacy, this is not provided here
                Reason = "Condemned via GRN Deposit",
                GRNNumber = dto.GRNNumber,
                GRNDate = dto.GRNDate.HasValue ? ToUtc(dto.GRNDate) : null,
                GRNBillAmount = dto.GRNBillAmount,
                IsOldGrn = dto.IsOldGrn,
                GrnDocumentPath = grnDocPath,
                ReplacementVehicleId = dto.ReplacementVehicleId,
                HaveEnteredAllGRNsFullAmount = dto.HaveYouEnteredAllGRNsFullAmount,
                AuctionAmount = dto.AmountForSelectedVehicle, // Map amount for selected vehicle to AuctionAmount
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.VehicleCondemnations.Add(condemnation);

            if (dto.HaveYouEnteredAllGRNsFullAmount)
            {
                vehicle.CurrentStatus = "CONDEMNED";
                vehicle.UpdatedDate = DateTime.UtcNow;
            }

            // Link Replacement Vehicle
            if (dto.ReplacementVehicleId.HasValue && dto.ReplacementVehicleId.Value > 0)
            {
                var newVehicle = await _context.Vehicles.FindAsync(dto.ReplacementVehicleId.Value);
                if (newVehicle != null)
                {
                    // Optionally set a field indicating it's a replacement
                    // legacy DB had 'regnewagainstexistvehiclerefid'
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<backend.DTOs.Vehicle.FitnessCertificateDto>> GetFitnessCertificatesAsync(int vehicleId)
        {
            var certificates = await _context.VehicleNOCDetails
                .Where(n => n.VehicleInfoId == vehicleId && n.NOC_IssueDate != null)
                .OrderByDescending(n => n.NOC_IssueDate)
                .Select(n => new backend.DTOs.Vehicle.FitnessCertificateDto
                {
                    VehicleNOCDetailId = n.VehicleNOCDetailId,
                    CertificateIssuedDate = n.NOC_IssueDate.HasValue ? n.NOC_IssueDate.Value.ToString("dd/MM/yyyy") : "",
                    CertificateExpiryDate = n.NOC_ExpiryDate.HasValue ? n.NOC_ExpiryDate.Value.ToString("dd/MM/yyyy") : "",
                    Certificate = n.VehicleNOC ?? ""
                })
                .ToListAsync();

            return certificates;
        }

        public async Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetFuelBillsAsync(int vehicleId)
        {
            var bills = await _context.FuelMaintenances
                .Where(f => f.VehicleInfoId == vehicleId && (f.Action == "Fuel" || f.MaintenanceType == "Fuel" || f.Action.Contains("Petrol") || f.Action.Contains("Diesel")))
                .OrderByDescending(f => f.BillDate)
                .Select(f => new backend.DTOs.Vehicle.BillRecordDto
                {
                    RecordId = f.FuelMaintenanceId.ToString(),
                    ClaimNumber = f.ClaimNo ?? "N/A",
                    SubVoucherNo = f.SubVoucherNo ?? "N/A",
                    Date = f.BillDate.ToString("dd/MM/yyyy"),
                    Type = f.Action ?? "Fuel",
                    Amount = f.Amount,
                    OdometerReading = f.OdometerReading,
                    SanctionOrderNo = f.SanctionOrderNo ?? "N/A",
                    SanctionOrderDate = f.SanctionOrderDate.HasValue ? f.SanctionOrderDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    SanctionAuthority = f.SanctionAuthority ?? "N/A",
                    PermissionReceived = "Yes", // Placeholder for legacy mapping
                    VmsEntryDate = f.CreatedDate.ToString("dd/MM/yyyy"), // Legacy used ActionDate which mapped to PDate/CreatedDate
                    FuelConsumptionLitres = 0m, // Not strictly available in FuelMaintenance, requires join with FuelEntry, leaving 0 for now as stub
                    PermissionNoc = "" // Placeholder as it is not in FuelMaintenance table
                })
                .ToListAsync();

            return bills;
        }

        public async Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetMaintenanceBillsAsync(int vehicleId)
        {
            var bills = await _context.FuelMaintenances
                .Where(f => f.VehicleInfoId == vehicleId && f.Action != "Fuel" && f.MaintenanceType != "Fuel" && !f.Action.Contains("Petrol") && !f.Action.Contains("Diesel"))
                .OrderByDescending(f => f.BillDate)
                .Select(f => new backend.DTOs.Vehicle.BillRecordDto
                {
                    RecordId = f.FuelMaintenanceId.ToString(),
                    ClaimNumber = f.ClaimNo ?? "N/A",
                    SubVoucherNo = f.SubVoucherNo ?? "N/A",
                    Date = f.BillDate.ToString("dd/MM/yyyy"),
                    Type = f.Action ?? "Maintenance",
                    Amount = f.Amount,
                    OdometerReading = f.OdometerReading,
                    SanctionOrderNo = f.SanctionOrderNo ?? "N/A",
                    SanctionOrderDate = f.SanctionOrderDate.HasValue ? f.SanctionOrderDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    SanctionAuthority = f.SanctionAuthority ?? "N/A",
                    PermissionReceived = "Yes",
                    VmsEntryDate = f.CreatedDate.ToString("dd/MM/yyyy"),
                    PermissionNoc = ""
                })
                .ToListAsync();

            return bills;
        }

        public async Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetServiceBillsAsync(int vehicleId)
        {
            var bills = await _context.FuelMaintenances
                .Where(f => f.VehicleInfoId == vehicleId && f.Action == "Service")
                .OrderByDescending(f => f.BillDate)
                .Select(f => new backend.DTOs.Vehicle.BillRecordDto
                {
                    RecordId = f.FuelMaintenanceId.ToString(),
                    ClaimNumber = f.ClaimNo ?? "N/A",
                    SubVoucherNo = f.SubVoucherNo ?? "N/A",
                    Date = f.BillDate.ToString("dd/MM/yyyy"),
                    Type = f.Action ?? "Service",
                    Amount = f.Amount,
                    OdometerReading = f.OdometerReading,
                    SanctionOrderNo = f.SanctionOrderNo ?? "N/A",
                    SanctionOrderDate = f.SanctionOrderDate.HasValue ? f.SanctionOrderDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    SanctionAuthority = f.SanctionAuthority ?? "N/A",
                    PermissionReceived = "Yes",
                    VmsEntryDate = f.CreatedDate.ToString("dd/MM/yyyy"),
                    PermissionNoc = ""
                })
                .ToListAsync();

            return bills;
        }

        public async Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetBatteryChangesAsync(int vehicleId)
        {
            var bills = await _context.FuelMaintenances
                .Where(f => f.VehicleInfoId == vehicleId && f.Action == "Battery Change")
                .OrderByDescending(f => f.BillDate)
                .Select(f => new backend.DTOs.Vehicle.BillRecordDto
                {
                    RecordId = f.FuelMaintenanceId.ToString(),
                    ClaimNumber = f.ClaimNo ?? "N/A",
                    SubVoucherNo = f.SubVoucherNo ?? "N/A",
                    Date = f.BillDate.ToString("dd/MM/yyyy"),
                    Type = f.Action ?? "Battery Change",
                    Amount = f.Amount,
                    OdometerReading = f.OdometerReading,
                    SanctionOrderNo = f.SanctionOrderNo ?? "N/A",
                    SanctionOrderDate = f.SanctionOrderDate.HasValue ? f.SanctionOrderDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    SanctionAuthority = f.SanctionAuthority ?? "N/A",
                    PermissionReceived = "Yes",
                    VmsEntryDate = f.CreatedDate.ToString("dd/MM/yyyy"),
                    PermissionNoc = ""
                })
                .ToListAsync();

            return bills;
        }

        public async Task<List<backend.DTOs.Vehicle.BillRecordDto>> GetTyreChangesAsync(int vehicleId)
        {
            var bills = await _context.FuelMaintenances
                .Where(f => f.VehicleInfoId == vehicleId && f.Action == "Tyre Change")
                .OrderByDescending(f => f.BillDate)
                .Select(f => new backend.DTOs.Vehicle.BillRecordDto
                {
                    RecordId = f.FuelMaintenanceId.ToString(),
                    ClaimNumber = f.ClaimNo ?? "N/A",
                    SubVoucherNo = f.SubVoucherNo ?? "N/A",
                    Date = f.BillDate.ToString("dd/MM/yyyy"),
                    Type = f.Action ?? "Tyre Change",
                    Amount = f.Amount,
                    OdometerReading = f.OdometerReading,
                    SanctionOrderNo = f.SanctionOrderNo ?? "N/A",
                    SanctionOrderDate = f.SanctionOrderDate.HasValue ? f.SanctionOrderDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    SanctionAuthority = f.SanctionAuthority ?? "N/A",
                    PermissionReceived = "Yes",
                    VmsEntryDate = f.CreatedDate.ToString("dd/MM/yyyy"),
                    PermissionNoc = ""
                })
                .ToListAsync();

            return bills;
        }

        public async Task<List<backend.DTOs.Vehicle.TransferHistoryDto>> GetTransferHistoryAsync(int vehicleId)
        {
            var history = await _context.VehicleTransfers
                .Where(t => t.VehicleId == vehicleId)
                .Include(t => t.FromOffice)
                    .ThenInclude(o => o.Department)
                .OrderByDescending(t => t.TransferDate)
                .Select(t => new backend.DTOs.Vehicle.TransferHistoryDto
                {
                    VehicleTransferredOn = t.TransferDate.ToString("dd/MM/yyyy"),
                    VerifiedOn = t.VerificationDate.HasValue ? t.VerificationDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    PreviousOffice = t.FromOffice.OfficeName,
                    PreviousDepartment = t.FromOffice.Department != null ? t.FromOffice.Department.DeptName : "N/A",
                    AllocationType = t.FromAllocationType,
                    OfficerName = t.FromOfficerName,
                    OfficerDesignation = t.FromDesignationName,
                    Remarks = t.Remarks,
                    PreviousDdo = t.FromDdoCode,
                    NewDdo = t.ToDdoCode
                })
                .ToListAsync();

            return history;
        }

        public async Task<List<VehicleResponseDto>> GetVehiclesByDdoAsync(string ddoCode)
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Manufacturer)
                .Include(v => v.Model)
                .Include(v => v.VehicleType)
                .Include(v => v.Office)
                .Include(v => v.VehicleCondemnations)
                .Where(v => v.IsActive && 
                            (v.DDOId == ddoCode || 
                             (v.DDOId == "" && _context.Users.Any(u => u.UserId.ToString() == v.CreatedBy && u.DDOCode == ddoCode))
                            ) && 
                            v.verificationstatus == 1)
                .ToListAsync();

            return vehicles.Select(MapToResponseDto).ToList();
        }

        private VehicleResponseDto MapToResponseDto(VehicleInfo v)
        {
            var latestCondemnation = v.VehicleCondemnations?.OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            return new VehicleResponseDto
            {
                Id = v.VehicleInfoId,
                RegistrationNumber = v.VehicleNumber,
                ChassisNumber = v.EngineChasisNumber,
                Manufacturer = v.Manufacturer?.ManufacturerName ?? string.Empty,
                Model = v.Model?.ModelName ?? string.Empty,
                VehicleType = v.VehicleType?.VehicleTypeName ?? string.Empty,
                DdoCode = v.DDOId,
                VehiclePhoto = v.VehiclePhotoPath,
                RegistrationCertificate = v.RegistrationCertificatePath,
                FdApproval = latestCondemnation != null ? latestCondemnation.FdApprovalDocPath : v.FdApprovalPath,
                CondemnationReplacementRegNo = latestCondemnation != null ? latestCondemnation.ReplacementVehicleRegNo : string.Empty,
                CondemnationReplacementChassisNo = latestCondemnation != null ? latestCondemnation.ReplacementVehicleChassisNo : string.Empty,
                FleetStrengthLetter = v.FleetStrengthLetterPath,
                OfficeName = v.Office?.OfficeName ?? string.Empty,
                OfficeAddress = v.Office?.OfficeAddress ?? string.Empty,
                Designation = v.Designation?.DesignationName ?? string.Empty,
                Department = v.Department?.DeptName ?? string.Empty,
                District = v.Office?.District?.DistrictName ?? string.Empty,
                Tehsil = v.Office?.Tehsil?.TehsilName ?? string.Empty,
                OfficerName = v.OfficerName,
                HrmsCode = v.HRMSCode,
                CurrentStatus = v.CurrentStatus,
                VerificationStatus = v.verificationstatus,
                VerificationComments = v.Comments,
                VerificationDate = v.VerificationDate,
                VehicleAllocationType = v.AllocationType,
                ProjectName = v.Project?.ProjectName ?? string.Empty,
                VehicleCost = v.VehicleCost != null ? Convert.ToDecimal(v.VehicleCost) : null,
                PurchaseDate = v.VehiclePurchaseDate,
                ManufactureYear = v.ManufactureYear.ToString(),
                SeatingCapacity = int.TryParse(v.SeatingCapacity, out int cap) ? cap : null,
                FuelUsed = v.FuelUsed,
                DriverType = v.DriverType,
                DriverName = v.DriverName,
                DriverContactNumber = v.DriverContactNo,
                ContractorName = v.ContractorName,
                ContractorContactNumber = v.ContractorContactNo,
                RequisitionDeptName = v.RequisitionDepartment?.DeptName ?? string.Empty,
                RequisitionOfficeName = v.RequisitionOffice?.OfficeName ?? string.Empty,
                
                // Nodal Officer
                NodalOfficerName = v.NodalOfficerName,
                NodalOfficerEmail = v.NodalOfficerEmail,
                NodalOfficerMobileNo = v.NodalOfficerMobileNo,
                TreasuryType = v.TreasuryType,
                PDate = v.PDate,
                ReadingUptodate = v.ReadingUptodate,
                FinancialYearReading = v.FinancialYearReading,
                KmsCovered = v.KM_30062017,
                FuelCostLast3Months = v.FuelConsumptionCost != null ? Convert.ToDecimal(v.FuelConsumptionCost) : null,
                FuelLitresLast3Months = v.FuelConsumptionLitres != null ? Convert.ToDecimal(v.FuelConsumptionLitres) : null,
                MaintenanceCostLast3Months = v.LastThreeYearsMaintenanceCost != null ? Convert.ToDecimal(v.LastThreeYearsMaintenanceCost) : null,
                MaintenenceDuration = v.MaintenenceDuration,
                IsTyreOriginal = v.IsTyreOriginal == true ? "Yes" : "No",
                TyreChangedDate = v.LastTyreChangedDate,
                TyreChangedMeterReading = v.LastTyreChangedKM,
                FitnessUpto = v.FitnessUpto,
                CreatedAt = v.CreatedDate,
                UpdatedAt = v.UpdatedDate,
                CreatedByUserId = int.TryParse(v.CreatedBy, out int uid) ? uid : 0
            };
        }
    }
}
