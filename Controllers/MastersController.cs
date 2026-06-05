using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs.Masters;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MastersController : ControllerBase
    {
        private readonly IMasterService _masterService;

        public MastersController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        [HttpGet("departments")]
        public async Task<ActionResult<List<DepartmentResponseDto>>> GetDepartments()
        {
            return Ok(await _masterService.GetAllDepartmentsAsync());
        }

        [HttpGet("departments/by-user")]
        public async Task<ActionResult<List<DepartmentResponseDto>>> GetDepartmentsByUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var deptIdClaim = User.FindFirst("departmentId")?.Value;

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            int? userDeptId = !string.IsNullOrEmpty(deptIdClaim) ? int.Parse(deptIdClaim) : null;

            // Admin sees all departments
            if (roleClaim == "ADMN")
            {
                return Ok(await _masterService.GetAllDepartmentsAsync());
            }

            // Other roles see only their assigned department(s)
            var departments = await _masterService.GetDepartmentsByUserAsync(userId, userDeptId);
            return Ok(departments);
        }

        [HttpGet("districts")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetDistricts()
        {
            return Ok(await _masterService.GetAllDistrictsAsync());
        }

        [HttpGet("districts/by-user")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetDistrictsByUser()
        {
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var distIdClaim = User.FindFirst("districtId")?.Value;

            if (roleClaim == null)
                return Unauthorized();

            // Admin sees all districts
            if (roleClaim == "ADMN")
            {
                return Ok(await _masterService.GetAllDistrictsAsync());
            }

            // Other users see only their own district from JWT claims
            int? userDistId = !string.IsNullOrEmpty(distIdClaim) ? int.Parse(distIdClaim) : null;
            var districts = await _masterService.GetDistrictsByUserAsync(userDistId);
            return Ok(districts);
        }

        [HttpGet("tehsils/{districtId}")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetTehsils(int districtId)
        {
            return Ok(await _masterService.GetAllTehsilsAsync(districtId));
        }

        [HttpGet("tehsils/by-user")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetTehsilsByUser()
        {
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var distIdClaim = User.FindFirst("districtId")?.Value;

            if (roleClaim == null)
                return Unauthorized();

            // Admin sees all tehsils (no district filter)
            if (roleClaim == "ADMN")
            {
                return Ok(await _masterService.GetAllTehsilsAsync());
            }

            // Other users see tehsils only in their own district
            int? userDistId = !string.IsNullOrEmpty(distIdClaim) ? int.Parse(distIdClaim) : null;
            if (userDistId.HasValue && userDistId.Value > 0)
            {
                return Ok(await _masterService.GetAllTehsilsAsync(userDistId.Value));
            }

            // Fallback: return all tehsils if no district assigned
            return Ok(await _masterService.GetAllTehsilsAsync());
        }

        [HttpGet("office-types")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetOfficeTypes()
        {
            return Ok(await _masterService.GetAllOfficeTypesAsync());
        }

        [HttpGet("offices")]
        public async Task<ActionResult<List<OfficeResponseDto>>> GetOffices([FromQuery] int? departmentId)
        {
            // Extract user info from JWT claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var deptIdClaim = User.FindFirst("departmentId")?.Value;
            var distIdClaim = User.FindFirst("districtId")?.Value;

            // If not authenticated, return unauthorized
            if (userIdClaim == null || roleClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            int? userDeptId = !string.IsNullOrEmpty(deptIdClaim) ? int.Parse(deptIdClaim) : null;
            int? userDistId = !string.IsNullOrEmpty(distIdClaim) ? int.Parse(distIdClaim) : null;

            // If a specific departmentId is explicitly requested by the client,
            // return ALL offices associated with that department (bypassing user context filtering, 
            // since this is needed for transferring vehicles to external offices).
            if (departmentId.HasValue)
            {
                var allDeptOffices = await _masterService.GetAllOfficesAsync(departmentId.Value);
                return Ok(allDeptOffices);
            }

            // Otherwise, fall back to role-based filtering for the logged-in user context
            var offices = await _masterService.GetOfficesByUserContextAsync(
                userId, 
                roleClaim, 
                userDeptId, 
                userDistId
            );

            return Ok(offices);
        }

        [HttpGet("offices/{id}")]
        public async Task<ActionResult<OfficeResponseDto>> GetOffice(int id)
        {
            var office = await _masterService.GetOfficeByIdAsync(id);
            if (office == null) return NotFound();
            return Ok(office);
        }

        [HttpPost("offices")]
        public async Task<ActionResult<OfficeResponseDto>> CreateOffice(OfficeRequestDto request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var distIdClaim = User.FindFirst("districtId")?.Value;

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized();

            // Legacy behavior: Only DDO can create offices
            if (roleClaim != "DDO" && roleClaim != "ADMN")
            {
                return Forbid();
            }

            int? userDistId = !string.IsNullOrEmpty(distIdClaim) ? int.Parse(distIdClaim) : null;

            // For non-admin: Use districtId from JWT (legacy behavior)
            // For admin: Use districtId from request
            int? finalDistrictId = roleClaim == "ADMN" ? request.DistrictId : userDistId;

            // Build request with enforced values
            var enrichedRequest = request with
            {
                UserId = userIdClaim,
                DistrictId = finalDistrictId
            };

            var office = await _masterService.AddOfficeAsync(enrichedRequest);
            return CreatedAtAction(nameof(GetOffice), new { id = office.Id }, office);
        }

        [HttpPut("offices/{id}")]
        public async Task<ActionResult<OfficeResponseDto>> UpdateOffice(int id, OfficeRequestDto request)
        {
            var office = await _masterService.UpdateOfficeAsync(id, request);
            if (office == null) return NotFound();
            return Ok(office);
        }

        [HttpDelete("offices/{id}")]
        public async Task<ActionResult> DeleteOffice(int id)
        {
            var result = await _masterService.DeleteOfficeAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("designations")]
        public async Task<ActionResult<List<DesignationResponseDto>>> GetDesignations([FromQuery] int? officeId, [FromQuery] int? departmentId)
        {
            if (departmentId.HasValue)
            {
                return Ok(await _masterService.GetDesignationsByDepartmentAsync(departmentId.Value));
            }
            return Ok(await _masterService.GetAllDesignationsAsync(officeId));
        }

        // NEW: Get designations filtered by user's department from JWT (legacy GetDesignationByDeptId equivalent)
        [HttpGet("designations/by-user")]
        public async Task<ActionResult<List<DesignationResponseDto>>> GetDesignationsByUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var deptIdClaim = User.FindFirst("departmentId")?.Value;

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized();

            // Admin sees all designations
            if (roleClaim == "ADMN")
            {
                return Ok(await _masterService.GetAllDesignationsAsync());
            }

            // Other users see designations only for their department (matching legacy logic)
            int? userDeptId = !string.IsNullOrEmpty(deptIdClaim) ? int.Parse(deptIdClaim) : null;
            
            if (userDeptId.HasValue && userDeptId.Value > 0)
            {
                return Ok(await _masterService.GetDesignationsByDepartmentAsync(userDeptId.Value));
            }

            // Fallback: return all designations if no department assigned
            return Ok(await _masterService.GetAllDesignationsAsync());
        }

        [HttpGet("manufacturers")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetManufacturers()
        {
            return Ok(await _masterService.GetAllManufacturersAsync());
        }

        [HttpGet("vehicle-types")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetVehicleTypes()
        {
            return Ok(await _masterService.GetAllVehicleTypesAsync());
        }

        [HttpGet("vehicle-models")]
        public async Task<ActionResult<List<VehicleModelResponseDto>>> GetAllVehicleModels()
        {
            return Ok(await _masterService.GetAllVehicleModelsAsync());
        }

        [HttpGet("vehicle-models/{manufacturerId}")]
        public async Task<ActionResult<List<DropdownResponseDto>>> GetVehicleModels(int manufacturerId)
        {
            return Ok(await _masterService.GetVehicleModelsByManufacturerAsync(manufacturerId));
        }

        [HttpGet("officers")]
        public async Task<ActionResult<List<OfficerResponseDto>>> GetOfficers()
        {
            return Ok(await _masterService.GetAllOfficersAsync());
        }

        [HttpGet("officers/{id}")]
        public async Task<ActionResult<OfficerResponseDto>> GetOfficer(int id)
        {
            var officer = await _masterService.GetOfficerByIdAsync(id);
            if (officer == null) return NotFound();
            return Ok(officer);
        }

        [HttpPost("officers")]
        public async Task<ActionResult<OfficerResponseDto>> CreateOfficer(OfficerRequestDto request)
        {
            var officer = await _masterService.AddOfficerAsync(request);
            return CreatedAtAction(nameof(GetOfficer), new { id = officer.Id }, officer);
        }

        [HttpPut("officers/{id}")]
        public async Task<ActionResult<OfficerResponseDto>> UpdateOfficer(int id, OfficerRequestDto request)
        {
            var officer = await _masterService.UpdateOfficerAsync(id, request);
            if (officer == null) return NotFound();
            return Ok(officer);
        }

        [HttpDelete("officers/{id}")]
        public async Task<ActionResult> DeleteOfficer(int id)
        {
            var result = await _masterService.DeleteOfficerAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("projects")]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetProjects([FromQuery] int? departmentId)
        {
            return Ok(await _masterService.GetAllProjectsAsync(departmentId));
        }

        [HttpGet("projects/{id}")]
        public async Task<ActionResult<ProjectResponseDto>> GetProject(int id)
        {
            var project = await _masterService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpPost("projects")]
        public async Task<ActionResult<ProjectResponseDto>> CreateProject(ProjectRequestDto request)
        {
            var project = await _masterService.AddProjectAsync(request);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpPut("projects/{id}")]
        public async Task<ActionResult<ProjectResponseDto>> UpdateProject(int id, ProjectRequestDto request)
        {
            var project = await _masterService.UpdateProjectAsync(id, request);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpDelete("projects/{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            var result = await _masterService.DeleteProjectAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("verify-hrms/{hrmsCode}")]
        public async Task<ActionResult<HrmsResponseDto>> VerifyHrms(string hrmsCode)
        {
            var result = await _masterService.VerifyHRMSCodeAsync(hrmsCode);
            if (result == null) return NotFound(new { success = false, message = "Unable to verify HRMS code" });
            return Ok(result);
        }

        // --- Department Admin ---
        [HttpPost("departments")]
        public async Task<ActionResult<DepartmentResponseDto>> CreateDepartment(CreateDepartmentDto request)
        {
            return Ok(await _masterService.AddDepartmentAsync(request));
        }

        [HttpPut("departments/{id}")]
        public async Task<ActionResult<DepartmentResponseDto>> UpdateDepartment(int id, UpdateDepartmentDto request)
        {
            var result = await _masterService.UpdateDepartmentAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("departments/{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            var result = await _masterService.DeleteDepartmentAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- District Admin ---
        [HttpPost("districts")]
        public async Task<ActionResult<DropdownResponseDto>> CreateDistrict(DistrictRequestDto request)
        {
            return Ok(await _masterService.AddDistrictAsync(request));
        }

        [HttpPut("districts/{id}")]
        public async Task<ActionResult<DropdownResponseDto>> UpdateDistrict(int id, DistrictRequestDto request)
        {
            var result = await _masterService.UpdateDistrictAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("districts/{id}")]
        public async Task<ActionResult> DeleteDistrict(int id)
        {
            var result = await _masterService.DeleteDistrictAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Tehsil Admin ---
        [HttpPost("tehsils")]
        public async Task<ActionResult<DropdownResponseDto>> CreateTehsil(TehsilRequestDto request)
        {
            return Ok(await _masterService.AddTehsilAsync(request));
        }

        [HttpPut("tehsils/{id}")]
        public async Task<ActionResult<DropdownResponseDto>> UpdateTehsil(int id, TehsilRequestDto request)
        {
            var result = await _masterService.UpdateTehsilAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("tehsils/{id}")]
        public async Task<ActionResult> DeleteTehsil(int id)
        {
            var result = await _masterService.DeleteTehsilAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Vehicle Type Admin ---
        [HttpPost("vehicle-types")]
        public async Task<ActionResult<DropdownResponseDto>> CreateVehicleType(VehicleTypeRequestDto request)
        {
            return Ok(await _masterService.AddVehicleTypeAsync(request));
        }

        [HttpPut("vehicle-types/{id}")]
        public async Task<ActionResult<DropdownResponseDto>> UpdateVehicleType(int id, VehicleTypeRequestDto request)
        {
            var result = await _masterService.UpdateVehicleTypeAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("vehicle-types/{id}")]
        public async Task<ActionResult> DeleteVehicleType(int id)
        {
            var result = await _masterService.DeleteVehicleTypeAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Manufacturer Admin ---
        [HttpPost("manufacturers")]
        public async Task<ActionResult<DropdownResponseDto>> CreateManufacturer(ManufacturerRequestDto request)
        {
            return Ok(await _masterService.AddManufacturerAsync(request));
        }

        [HttpPut("manufacturers/{id}")]
        public async Task<ActionResult<DropdownResponseDto>> UpdateManufacturer(int id, ManufacturerRequestDto request)
        {
            var result = await _masterService.UpdateManufacturerAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("manufacturers/{id}")]
        public async Task<ActionResult> DeleteManufacturer(int id)
        {
            var result = await _masterService.DeleteManufacturerAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Office Type Admin ---
        [HttpPost("office-types")]
        public async Task<ActionResult<DropdownResponseDto>> CreateOfficeType(OfficeTypeRequestDto request)
        {
            return Ok(await _masterService.AddOfficeTypeAsync(request));
        }

        [HttpPut("office-types/{id}")]
        public async Task<ActionResult<DropdownResponseDto>> UpdateOfficeType(int id, OfficeTypeRequestDto request)
        {
            var result = await _masterService.UpdateOfficeTypeAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("office-types/{id}")]
        public async Task<ActionResult> DeleteOfficeType(int id)
        {
            var result = await _masterService.DeleteOfficeTypeAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Allocation Admin ---
        [HttpPost("allocations")]
        public async Task<ActionResult<DropdownResponseDto>> CreateAllocation(AllocationRequestDto request)
        {
            return Ok(await _masterService.AddAllocationAsync(request));
        }

        [HttpPut("allocations/{id}")]
        public async Task<ActionResult<DropdownResponseDto>> UpdateAllocation(int id, AllocationRequestDto request)
        {
            var result = await _masterService.UpdateAllocationAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("allocations/{id}")]
        public async Task<ActionResult> DeleteAllocation(int id)
        {
            var result = await _masterService.DeleteAllocationAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Model Admin ---
        [HttpPost("vehicle-models")]
        public async Task<ActionResult<VehicleModelResponseDto>> CreateModel(VehicleModelRequestDto request)
        {
            return Ok(await _masterService.AddVehicleModelAsync(request));
        }

        [HttpPut("vehicle-models/{id}")]
        public async Task<ActionResult<VehicleModelResponseDto>> UpdateModel(int id, VehicleModelRequestDto request)
        {
            var result = await _masterService.UpdateVehicleModelAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("vehicle-models/{id}")]
        public async Task<ActionResult> DeleteModel(int id)
        {
            var result = await _masterService.DeleteVehicleModelAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Designation Admin ---
        [HttpPost("designations")]
        public async Task<ActionResult<DesignationResponseDto>> CreateDesignation(DesignationRequestDto request)
        {
            return Ok(await _masterService.AddDesignationAsync(request));
        }

        [HttpPut("designations/{id}")]
        public async Task<ActionResult<DesignationResponseDto>> UpdateDesignation(int id, DesignationRequestDto request)
        {
            var result = await _masterService.UpdateDesignationAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("designations/{id}")]
        public async Task<ActionResult> DeleteDesignation(int id)
        {
            var result = await _masterService.DeleteDesignationAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("inventory")]
        public async Task<ActionResult<List<InventoryDropdownDto>>> GetInventoryItems()
        {
            return Ok(await _masterService.GetAllInventoryItemsAsync());
        }

        // --- FleetStrength Admin ---
        [HttpGet("fleet-strengths")]
        public async Task<ActionResult<List<FleetStrengthResponseDto>>> GetFleetStrengths([FromQuery] int? departmentId)
        {
            return Ok(await _masterService.GetAllFleetStrengthsAsync(departmentId));
        }

        [HttpPost("fleet-strengths")]
        public async Task<ActionResult<FleetStrengthResponseDto>> CreateFleetStrength(FleetStrengthRequestDto request)
        {
            return Ok(await _masterService.AddFleetStrengthAsync(request));
        }

        [HttpPut("fleet-strengths/{id}")]
        public async Task<ActionResult<FleetStrengthResponseDto>> UpdateFleetStrength(int id, FleetStrengthRequestDto request)
        {
            var result = await _masterService.UpdateFleetStrengthAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("fleet-strengths/{id}")]
        public async Task<ActionResult> DeleteFleetStrength(int id)
        {
            var result = await _masterService.DeleteFleetStrengthAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // --- StoreItems Admin ---
        [HttpGet("store-items")]
        public async Task<ActionResult<List<StoreItemResponseDto>>> GetStoreItems()
        {
            return Ok(await _masterService.GetAllStoreItemsAsync());
        }

        [HttpPost("store-items")]
        public async Task<ActionResult<StoreItemResponseDto>> CreateStoreItem(StoreItemRequestDto request)
        {
            return Ok(await _masterService.AddStoreItemAsync(request));
        }

        [HttpPut("store-items/{id}")]
        public async Task<ActionResult<StoreItemResponseDto>> UpdateStoreItem(int id, StoreItemRequestDto request)
        {
            var result = await _masterService.UpdateStoreItemAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("store-items/{id}")]
        public async Task<ActionResult> DeleteStoreItem(int id)
        {
            var result = await _masterService.DeleteStoreItemAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
