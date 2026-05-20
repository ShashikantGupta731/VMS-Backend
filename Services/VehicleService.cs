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
                DDOId = string.Empty,
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

        public async Task<bool> CondemnVehicleAsync(CondemnVehicleDto dto, int userId)
        {
            var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
            if (vehicle == null) return false;

            // 1. Update Vehicle Status
            vehicle.CurrentStatus = "CONDEMNED";
            vehicle.UpdatedDate = DateTime.UtcNow;

            // 2. Save Condemnation Order File
            string orderPath = "";
            if (dto.CondemnationOrderFile != null)
            {
                orderPath = await _fileService.SaveFileAsync(dto.CondemnationOrderFile, "vehicles/condemnation") ?? "";
            }

            // 3. Create Condemnation Record
            var condemnation = new VehicleCondemnation
            {
                VehicleId = dto.VehicleId,
                CondemnationDate = dto.CondemnationDate,
                CondemnationOrderNumber = dto.CondemnationOrderNumber,
                CondemnationOrderPath = orderPath,
                Reason = dto.Reason,
                AuctionStatus = dto.AuctionStatus ?? "Pending",
                AuctionDate = dto.AuctionDate,
                AuctionAmount = dto.AuctionAmount,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.VehicleCondemnations.Add(condemnation);
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
                .Where(v => v.IsActive && v.DDOId == ddoCode && v.verificationstatus == 1)
                .ToListAsync();

            return vehicles.Select(MapToResponseDto).ToList();
        }

        private VehicleResponseDto MapToResponseDto(VehicleInfo v)
        {
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
                FdApproval = v.FdApprovalPath,
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
