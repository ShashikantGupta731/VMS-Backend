using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Core;
using backend.DTOs.VMSMobileApp;
using backend.Services;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public MobileController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost("DriverLatLong")]
        public async Task<ActionResult<OutDriverLatLongDto>> DriverLatLong([FromForm] InDriverLatLongDto param)
        {
            try
            {
                // Process image file if sent via multipart/form-data
                string? imgPath = param.OdometerImg; // Fallback to whatever string they sent (base64/legacy path)
                if (param.OdometerImgFile != null && param.OdometerImgFile.Length > 0)
                {
                    imgPath = await _fileService.SaveFileAsync(param.OdometerImgFile, "mobile/odometer");
                }

                TripDetail? trip = null;

                if (param.TripId > 0)
                {
                    trip = await _context.TripDetails.FirstOrDefaultAsync(t => t.TripId == param.TripId);
                }

                if (trip == null)
                {
                    // Create new trip
                    trip = new TripDetail
                    {
                        VehicleInfoId = param.VehicleInfoId,
                        Longitude = param.Longitude,
                        Latitude = param.Latitude,
                        DriverName = param.DriverName,
                        DriverPhone = param.DriverPhone,
                        OdometerReading = param.OdometerReading,
                        OdometerImgPath = imgPath,
                        TripCompleted = param.TripCompleted,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.TripDetails.Add(trip);
                }
                else
                {
                    // Update existing trip
                    trip.Longitude = param.Longitude;
                    trip.Latitude = param.Latitude;
                    trip.OdometerReading = param.OdometerReading;
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        trip.OdometerImgPath = imgPath;
                    }
                    trip.TripCompleted = param.TripCompleted;
                    trip.UpdatedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    result = new[]
                    {
                        new OutDriverLatLongDto
                        {
                            TripId = trip.TripId,
                            Success = true,
                            Msg = "Trip details saved successfully."
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = ex.Message });
            }
        }

        [HttpPost("GetVehicleEntryByDriverContact")]
        public async Task<ActionResult> GetVehicleEntryByDriverContact([FromBody] InGetVehicleEntryByDriverContactDto param)
        {
            try
            {
                // Find vehicle where driver phone matches
                // Include related tables for manufacturer and model
                var vehicle = await _context.Vehicles
                    .Include(v => v.Manufacturer)
                    .Include(v => v.Model)
                    .Include(v => v.VehicleType)
                    .Where(v => v.DriverContactNo == param.DriverContactNo)
                    .FirstOrDefaultAsync();

                if (vehicle == null)
                {
                    return NotFound(new { success = false, msg = "Unable to Retrieve Data" });
                }

                var result = new OutGetVehicleEntryByDriverContactDto
                {
                    VehicleInfoId = vehicle.VehicleInfoId,
                    ManufacturerName = vehicle.Manufacturer?.ManufacturerName,
                    ModelName = vehicle.Model?.ModelName,
                    VehicleNumber = vehicle.VehicleNumber,
                    FuelUsed = vehicle.VehicleType?.VehicleTypeName, // Assuming VehicleType maps to FuelUsed loosely, or map accordingly
                    DriverName = vehicle.DriverName,
                    DriverMobileNo = vehicle.DriverContactNo,
                    VehicleProofUploads = vehicle.VehiclePhotoPath // Or whichever proof upload field is needed
                };

                return Ok(new
                {
                    success = true,
                    result = new[] { result } // Wrapping in array to match legacy response format
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = ex.Message });
            }
        }
    }
}
