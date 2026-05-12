using backend.DTOs.Masters;

namespace backend.Services
{
    public interface IMasterService
    {
        Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync();
        Task<List<DepartmentResponseDto>> GetDepartmentsByUserAsync(int userId, int? userDeptId);
        Task<List<DropdownResponseDto>> GetAllDistrictsAsync();
        Task<List<DropdownResponseDto>> GetDistrictsByUserAsync(int? userDistId);
        Task<List<DropdownResponseDto>> GetAllTehsilsAsync(int? districtId = null);
        
        // Notice the optional departmentId filter!
        Task<List<OfficeResponseDto>> GetAllOfficesAsync(int? departmentId = null);
        Task<OfficeResponseDto?> GetOfficeByIdAsync(int id);
        Task<OfficeResponseDto> AddOfficeAsync(OfficeRequestDto request);
        Task<OfficeResponseDto?> UpdateOfficeAsync(int id, OfficeRequestDto request);
        Task<bool> DeleteOfficeAsync(int id);
        
        Task<List<DropdownResponseDto>> GetAllOfficeTypesAsync();
        
        Task<List<DesignationResponseDto>> GetAllDesignationsAsync(int? officeId = null);
        // NEW: Fetch designations directly by department ID (matching legacy GetDesignationByDeptId)
        Task<List<DesignationResponseDto>> GetDesignationsByDepartmentAsync(int departmentId);
        Task<List<DropdownResponseDto>> GetAllManufacturersAsync();
        Task<List<DropdownResponseDto>> GetAllVehicleTypesAsync();
        
        // We filter vehicle models by their parent manufacturer
        Task<List<DropdownResponseDto>> GetVehicleModelsByManufacturerAsync(int manufacturerId);

        // New: Detailed list of all models
        Task<List<VehicleModelResponseDto>> GetAllVehicleModelsAsync();

        Task<List<OfficerResponseDto>> GetAllOfficersAsync();
        Task<OfficerResponseDto?> GetOfficerByIdAsync(int id);
        Task<OfficerResponseDto> AddOfficerAsync(OfficerRequestDto request);
        Task<OfficerResponseDto?> UpdateOfficerAsync(int id, OfficerRequestDto request);
        Task<List<OfficeResponseDto>> GetOfficesByUserContextAsync(int userId, string userRole, int? userDepartmentId = null, int? userDistrictId = null);
        Task<bool> DeleteOfficerAsync(int id);

        Task<List<ProjectResponseDto>> GetAllProjectsAsync(int? departmentId = null);
        Task<ProjectResponseDto?> GetProjectByIdAsync(int id);
        Task<ProjectResponseDto> AddProjectAsync(ProjectRequestDto request);
        Task<ProjectResponseDto?> UpdateProjectAsync(int id, ProjectRequestDto request);
        Task<bool> DeleteProjectAsync(int id);

        // Models
        Task<VehicleModelResponseDto> AddVehicleModelAsync(VehicleModelRequestDto request);
        Task<VehicleModelResponseDto?> UpdateVehicleModelAsync(int id, VehicleModelRequestDto request);
        Task<bool> DeleteVehicleModelAsync(int id);

        // Designations
        Task<DesignationResponseDto> AddDesignationAsync(DesignationRequestDto request);
        Task<DesignationResponseDto?> UpdateDesignationAsync(int id, DesignationRequestDto request);
        Task<bool> DeleteDesignationAsync(int id);

        Task<HrmsResponseDto?> VerifyHRMSCodeAsync(string hrmsCode);
    }
}

