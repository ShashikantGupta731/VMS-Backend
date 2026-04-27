using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _context;

        public VehicleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto, int userId)
        {
            var vehicle = new Vehicle
            {
                PurchasedNewVehicle = dto.PurchasedNewVehicle,
                OfficeName = dto.OfficeName,
                CurrentStatus = dto.CurrentStatus,
                VehicleAllocationType = dto.VehicleAllocationType,
                Designation = dto.Designation,
                OfficerName = dto.OfficerName,
                HrmsCode = dto.HrmsCode,
                DriverType = dto.DriverType,
                DriverName = dto.DriverName,
                DriverContactNumber = dto.DriverContactNumber,
                ContractorName = dto.ContractorName,
                ContractorContactNumber = dto.ContractorContactNumber,
                Department = dto.Department,
                VehicleOwnerOffice = dto.VehicleOwnerOffice,
                RegistrationType = dto.RegistrationType,
                RegistrationNumber = dto.RegistrationNumber,
                ManufactureYear = dto.ManufactureYear,
                SeatingCapacity = dto.SeatingCapacity,
                VehicleType = dto.VehicleType,
                Manufacturer = dto.Manufacturer,
                Model = dto.Model,
                VehiclePhoto = dto.VehiclePhoto,
                RegistrationCertificate = dto.RegistrationCertificate,
                ChassisNumber = dto.ChassisNumber,
                VehicleCost = dto.VehicleCost,
                FuelUsed = dto.FuelUsed,
                PurchaseDate = dto.PurchaseDate,
                FitnessUpto = dto.FitnessUpto,
                KmsCovered = dto.KmsCovered,
                FuelCostLast3Months = dto.FuelCostLast3Months,
                FuelLitresLast3Months = dto.FuelLitresLast3Months,
                MaintenanceCostLast3Months = dto.MaintenanceCostLast3Months,
                IsTyreOriginal = dto.IsTyreOriginal,
                TyreChangedDate = dto.TyreChangedDate,
                TyreChangedMeterReading = dto.TyreChangedMeterReading,
                CreatedByUserId = userId
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return MapToResponseDto(vehicle);
        }

        public async Task<List<VehicleResponseDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.CreatedByUser)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return vehicles.Select(MapToResponseDto).ToList();
        }

        public async Task<VehicleResponseDto?> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.CreatedByUser)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return null;

            return MapToResponseDto(vehicle);
        }

        public async Task<VehicleResponseDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return null;

            vehicle.PurchasedNewVehicle = dto.PurchasedNewVehicle;
            vehicle.OfficeName = dto.OfficeName;
            vehicle.CurrentStatus = dto.CurrentStatus;
            vehicle.VehicleAllocationType = dto.VehicleAllocationType;
            vehicle.Designation = dto.Designation;
            vehicle.OfficerName = dto.OfficerName;
            vehicle.HrmsCode = dto.HrmsCode;
            vehicle.DriverType = dto.DriverType;
            vehicle.DriverName = dto.DriverName;
            vehicle.DriverContactNumber = dto.DriverContactNumber;
            vehicle.ContractorName = dto.ContractorName;
            vehicle.ContractorContactNumber = dto.ContractorContactNumber;
            vehicle.Department = dto.Department;
            vehicle.VehicleOwnerOffice = dto.VehicleOwnerOffice;
            vehicle.RegistrationType = dto.RegistrationType;
            vehicle.RegistrationNumber = dto.RegistrationNumber;
            vehicle.ManufactureYear = dto.ManufactureYear;
            vehicle.SeatingCapacity = dto.SeatingCapacity;
            vehicle.VehicleType = dto.VehicleType;
            vehicle.Manufacturer = dto.Manufacturer;
            vehicle.Model = dto.Model;
            vehicle.VehiclePhoto = dto.VehiclePhoto;
            vehicle.RegistrationCertificate = dto.RegistrationCertificate;
            vehicle.ChassisNumber = dto.ChassisNumber;
            vehicle.VehicleCost = dto.VehicleCost;
            vehicle.FuelUsed = dto.FuelUsed;
            vehicle.PurchaseDate = dto.PurchaseDate;
            vehicle.FitnessUpto = dto.FitnessUpto;
            vehicle.KmsCovered = dto.KmsCovered;
            vehicle.FuelCostLast3Months = dto.FuelCostLast3Months;
            vehicle.FuelLitresLast3Months = dto.FuelLitresLast3Months;
            vehicle.MaintenanceCostLast3Months = dto.MaintenanceCostLast3Months;
            vehicle.IsTyreOriginal = dto.IsTyreOriginal;
            vehicle.TyreChangedDate = dto.TyreChangedDate;
            vehicle.TyreChangedMeterReading = dto.TyreChangedMeterReading;
            vehicle.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(vehicle);
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        private VehicleResponseDto MapToResponseDto(Vehicle vehicle)
        {
            return new VehicleResponseDto
            {
                Id = vehicle.Id,
                PurchasedNewVehicle = vehicle.PurchasedNewVehicle,
                OfficeName = vehicle.OfficeName,
                CurrentStatus = vehicle.CurrentStatus,
                VehicleAllocationType = vehicle.VehicleAllocationType,
                Designation = vehicle.Designation,
                OfficerName = vehicle.OfficerName,
                HrmsCode = vehicle.HrmsCode,
                DriverType = vehicle.DriverType,
                DriverName = vehicle.DriverName,
                DriverContactNumber = vehicle.DriverContactNumber,
                ContractorName = vehicle.ContractorName,
                ContractorContactNumber = vehicle.ContractorContactNumber,
                Department = vehicle.Department,
                VehicleOwnerOffice = vehicle.VehicleOwnerOffice,
                RegistrationType = vehicle.RegistrationType,
                RegistrationNumber = vehicle.RegistrationNumber,
                ManufactureYear = vehicle.ManufactureYear,
                SeatingCapacity = vehicle.SeatingCapacity,
                VehicleType = vehicle.VehicleType,
                Manufacturer = vehicle.Manufacturer,
                Model = vehicle.Model,
                VehiclePhoto = vehicle.VehiclePhoto,
                RegistrationCertificate = vehicle.RegistrationCertificate,
                ChassisNumber = vehicle.ChassisNumber,
                VehicleCost = vehicle.VehicleCost,
                FuelUsed = vehicle.FuelUsed,
                PurchaseDate = vehicle.PurchaseDate,
                FitnessUpto = vehicle.FitnessUpto,
                KmsCovered = vehicle.KmsCovered,
                FuelCostLast3Months = vehicle.FuelCostLast3Months,
                FuelLitresLast3Months = vehicle.FuelLitresLast3Months,
                MaintenanceCostLast3Months = vehicle.MaintenanceCostLast3Months,
                IsTyreOriginal = vehicle.IsTyreOriginal,
                TyreChangedDate = vehicle.TyreChangedDate,
                TyreChangedMeterReading = vehicle.TyreChangedMeterReading,
                CreatedAt = vehicle.CreatedAt,
                UpdatedAt = vehicle.UpdatedAt,
                CreatedByUserId = vehicle.CreatedByUserId
            };
        }
    }
}
