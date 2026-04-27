using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IFileService _fileService;

        public VehiclesController(IVehicleService vehicleService, IFileService fileService)
        {
            _vehicleService = vehicleService;
            _fileService = fileService;
        }

        // POST /api/vehicles
        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromForm] IFormCollection formData)
        {
            // TODO: Get user ID from JWT token after authentication is implemented
            // For now, using a default user ID (1)
            int userId = 1;

            // Handle file uploads
            string vehiclePhotoPath = string.Empty;
            string registrationCertificatePath = string.Empty;

            var vehiclePhotoFile = formData.Files.GetFile("vehiclePhoto");
            if (vehiclePhotoFile != null && vehiclePhotoFile.Length > 0)
            {
                vehiclePhotoPath = await _fileService.SaveFileAsync(vehiclePhotoFile, "vehicles") ?? string.Empty;
            }

            var registrationCertificateFile = formData.Files.GetFile("registrationCertificate");
            if (registrationCertificateFile != null && registrationCertificateFile.Length > 0)
            {
                registrationCertificatePath = await _fileService.SaveFileAsync(registrationCertificateFile, "vehicles") ?? string.Empty;
            }

            // Map form data to DTO
            var dto = new CreateVehicleDto
            {
                PurchasedNewVehicle = formData["purchasedNewVehicle"].ToString(),
                OfficeName = formData["officeName"].ToString(),
                CurrentStatus = formData["currentStatus"].ToString(),
                VehicleAllocationType = formData["vehicleAllocationType"].ToString(),
                Designation = formData["designation"].ToString(),
                OfficerName = formData["officerName"].ToString(),
                HrmsCode = formData["hrmsCode"].ToString(),
                DriverType = formData["driverType"].ToString(),
                DriverName = formData["driverName"].ToString(),
                DriverContactNumber = formData["driverContactNumber"].ToString(),
                ContractorName = formData["contractorName"].ToString(),
                ContractorContactNumber = formData["contractorContactNumber"].ToString(),
                Department = formData["department"].ToString(),
                VehicleOwnerOffice = formData["vehicleOwnerOffice"].ToString(),
                RegistrationType = formData["registrationType"].ToString(),
                RegistrationNumber = formData["registrationNumber"].ToString(),
                ManufactureYear = formData["manufactureYear"].ToString(),
                SeatingCapacity = ParseNullableInt(formData["seatingCapacity"].ToString()),
                VehicleType = formData["vehicleType"].ToString(),
                Manufacturer = formData["manufacturer"].ToString(),
                Model = formData["model"].ToString(),
                VehiclePhoto = vehiclePhotoPath,
                RegistrationCertificate = registrationCertificatePath,
                ChassisNumber = formData["chassisNumber"].ToString(),
                VehicleCost = ParseNullableDecimal(formData["vehicleCost"].ToString()),
                FuelUsed = formData["fuelUsed"].ToString(),
                PurchaseDate = ParseNullableDateTime(formData["purchaseDate"].ToString()),
                FitnessUpto = ParseNullableDateTime(formData["fitnessUpto"].ToString()),
                KmsCovered = ParseNullableInt(formData["kmsCovered"].ToString()),
                FuelCostLast3Months = ParseNullableDecimal(formData["fuelCostLast3Months"].ToString()),
                FuelLitresLast3Months = ParseNullableDecimal(formData["fuelLitresLast3Months"].ToString()),
                MaintenanceCostLast3Months = ParseNullableDecimal(formData["maintenanceCostLast3Months"].ToString()),
                IsTyreOriginal = formData["isTyreOriginal"].ToString(),
                TyreChangedDate = ParseNullableDateTime(formData["tyreChangedDate"].ToString()),
                TyreChangedMeterReading = ParseNullableInt(formData["tyreChangedMeterReading"].ToString())
            };

            var result = await _vehicleService.CreateVehicleAsync(dto, userId);
            return CreatedAtAction(nameof(GetVehicleById), new { id = result.Id }, result);
        }

        // GET /api/vehicles
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        // GET /api/vehicles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(vehicle);
        }

        // PUT /api/vehicles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] UpdateVehicleDto dto)
        {
            var result = await _vehicleService.UpdateVehicleAsync(id, dto);
            if (result == null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(result);
        }

        // DELETE /api/vehicles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var success = await _vehicleService.DeleteVehicleAsync(id);
            if (!success)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(new { message = "Vehicle deleted successfully" });
        }

        private int? ParseNullableInt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (int.TryParse(value, out int result))
                return result;
            return null;
        }

        private decimal? ParseNullableDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (decimal.TryParse(value, out decimal result))
                return result;
            return null;
        }

        private DateTime? ParseNullableDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (DateTime.TryParse(value, out DateTime result))
                return result;
            return null;
        }
    }
}
