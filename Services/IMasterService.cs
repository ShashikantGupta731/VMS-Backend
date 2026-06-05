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

        // Districts
        Task<DropdownResponseDto> AddDistrictAsync(DistrictRequestDto request);
        Task<DropdownResponseDto?> UpdateDistrictAsync(int id, DistrictRequestDto request);
        Task<bool> DeleteDistrictAsync(int id);

        // Tehsils
        Task<DropdownResponseDto> AddTehsilAsync(TehsilRequestDto request);
        Task<DropdownResponseDto?> UpdateTehsilAsync(int id, TehsilRequestDto request);
        Task<bool> DeleteTehsilAsync(int id);

        // VehicleTypes
        Task<DropdownResponseDto> AddVehicleTypeAsync(VehicleTypeRequestDto request);
        Task<DropdownResponseDto?> UpdateVehicleTypeAsync(int id, VehicleTypeRequestDto request);
        Task<bool> DeleteVehicleTypeAsync(int id);

        // Manufacturers
        Task<DropdownResponseDto> AddManufacturerAsync(ManufacturerRequestDto request);
        Task<DropdownResponseDto?> UpdateManufacturerAsync(int id, ManufacturerRequestDto request);
        Task<bool> DeleteManufacturerAsync(int id);

        // OfficeTypes
        Task<DropdownResponseDto> AddOfficeTypeAsync(OfficeTypeRequestDto request);
        Task<DropdownResponseDto?> UpdateOfficeTypeAsync(int id, OfficeTypeRequestDto request);
        Task<bool> DeleteOfficeTypeAsync(int id);

        // Allocations
        Task<DropdownResponseDto> AddAllocationAsync(AllocationRequestDto request);
        Task<DropdownResponseDto?> UpdateAllocationAsync(int id, AllocationRequestDto request);
        Task<bool> DeleteAllocationAsync(int id);

        // Departments
        Task<DepartmentResponseDto> AddDepartmentAsync(CreateDepartmentDto request);
        Task<DepartmentResponseDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto request);
        Task<bool> DeleteDepartmentAsync(int id);

        // Models
        Task<VehicleModelResponseDto> AddVehicleModelAsync(VehicleModelRequestDto request);
        Task<VehicleModelResponseDto?> UpdateVehicleModelAsync(int id, VehicleModelRequestDto request);
        Task<bool> DeleteVehicleModelAsync(int id);

        // Designations
        Task<DesignationResponseDto> AddDesignationAsync(DesignationRequestDto request);
        Task<DesignationResponseDto?> UpdateDesignationAsync(int id, DesignationRequestDto request);
        Task<bool> DeleteDesignationAsync(int id);

        Task<HrmsResponseDto?> VerifyHRMSCodeAsync(string hrmsCode);
        Task<List<InventoryDropdownDto>> GetAllInventoryItemsAsync();

        // FleetStrength
        Task<List<FleetStrengthResponseDto>> GetAllFleetStrengthsAsync(int? deptId = null, int? districtId = null, int? vehicleTypeId = null);
        Task<FleetStrengthResponseDto?> GetFleetStrengthByIdAsync(int id);
        Task<FleetStrengthResponseDto> AddFleetStrengthAsync(FleetStrengthRequestDto request);
        Task<FleetStrengthResponseDto?> UpdateFleetStrengthAsync(int id, FleetStrengthRequestDto request);
        Task<bool> DeleteFleetStrengthAsync(int id);

        // StoreItems (InventoryItem)
        Task<List<StoreItemResponseDto>> GetAllStoreItemsAsync();
        Task<StoreItemResponseDto?> GetStoreItemByIdAsync(int id);
        Task<StoreItemResponseDto> AddStoreItemAsync(StoreItemRequestDto request);
        Task<StoreItemResponseDto?> UpdateStoreItemAsync(int id, StoreItemRequestDto request);
        Task<bool> DeleteStoreItemAsync(int id);
    }
}

