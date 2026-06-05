namespace backend.DTOs.Masters
{
    // A generic DTO for simple dropdowns (VehicleType, etc.)
    public record DropdownResponseDto(int Id, string Name);
    
    public record InventoryDropdownDto(int Id, string Name, bool IsModelRequired);
    
    // Department-specific DTO with legacy-compatible field names
    public record DepartmentResponseDto(
        int DeptId, 
        string DeptName, 
        string DeptAbbre, 
        bool? Enabled, 
        DateTime? PDate,
        DateTime? TDate
    );
    
    public record CreateDepartmentDto(
        string DepartmentName, 
        string DepartmentCode, 
        bool Enabled
    );
    
    public record UpdateDepartmentDto(
        int DeptId, 
        string DepartmentName, 
        string DepartmentCode, 
        bool Enabled
    );
    
    // A specific DTO for Offices since they have extra info
    public record OfficeResponseDto(
        int Id, 
        string OfficeName, 
        string OfficeAddress,
        int? DeptId,
        string DepartmentName,
        int? DistrictId,
        string DistrictName,
        int? TehsilId,
        string? TehsilName,
        string OfficeAbbreviation,
        int OfficeTypeId,
        string OfficeTypeOther,
        string UserId,
        bool Enabled
    );


    
    // Detailed DTO for Designations
    public record DesignationResponseDto(
        int DesignationId, 
        string DesignationName, 
        string DepartmentName,
        string DesignationType,
        int PetrolFuelLimit,
        int DieselFuelLimit,
        int PetrolMaintenanceLimit,
        int DieselMaintenanceLimit
    );

    // Detailed DTO for Vehicle Models
    public record VehicleModelResponseDto(
        int Id, 
        string ModelName, 
        string ManufacturerName, 
        int SeatingCapacity, 
        string VehicleType
    );

    // Detailed DTO for Officers
    public record OfficerResponseDto(
        int Id,
        string FirstName,
        string LastName,
        string OfficerName,

        string HRMSCode,
        int? DesignationId,
        string? DesignationName,
        int? OfficeId,
        string? OfficeName,
        decimal PetrolFuelLimit,
        decimal DieselFuelLimit,
        decimal PetrolMaintenanceLimit,
        decimal DieselMaintenanceLimit,
        string? Remarks
    );

    // DTO for Projects
    public record ProjectResponseDto(
        int Id, 
        string Name, 
        int DepartmentId,
        decimal PetrolFuelLimit,
        decimal PetrolMaintenanceLimit
    );

    // --- Request DTOs ---

    public record OfficeRequestDto(
        int OfficeId,
        string OfficeName,
        string OfficeAddress,
        int DeptId,
        int? DistrictId,          // Can come from frontend or JWT (controller will enforce)
        int? TehsilId,
        string OfficeAbbreviation,
        int OfficeTypeId,
        string OfficeTypeOther,
        string UserId
    );

    public record OfficerRequestDto(
        string FirstName,
        string LastName,
        string HRMSCode,
        int? DesignationId,
        int? OfficeId,
        decimal PetrolFuelLimit,
        decimal DieselFuelLimit,
        decimal PetrolMaintenanceLimit,
        decimal DieselMaintenanceLimit,
        string? Remarks
    );

    public record ProjectRequestDto(
        string Name,
        int DepartmentId,
        decimal PetrolFuelLimit,
        decimal PetrolMaintenanceLimit
    );

    public record VehicleModelRequestDto(
        string ModelName,
        int ManufacturerId,
        int SeatingCapacity,
        int VehicleTypeId
    );

    public record DistrictRequestDto(
        string DistrictName,
        string DistAbbre,
        bool IsActive
    );

    public record TehsilRequestDto(
        string TehsilName,
        int DistrictId,
        bool IsActive
    );

    public record VehicleTypeRequestDto(
        string VehicleTypeName,
        string VehicleTypeExample,
        int VehicleLifeKM,
        int VehicleLifeYears,
        bool IsActive
    );

    public record ManufacturerRequestDto(
        string ManufacturerName,
        bool IsActive
    );

    public record OfficeTypeRequestDto(
        string OfficeTypeName
    );

    public record AllocationRequestDto(
        string AllocationTypeName
    );

    public record DesignationRequestDto(
        string Name,
        int DepartmentId,
        string DesignationType,
        int PetrolFuelLimit,
        int DieselFuelLimit,
        int PetrolMaintenanceLimit,
        int DieselMaintenanceLimit
    );

    // HRMS Integration DTOs
    public record EmpDetailDto(
        string empcd,
        string empname,
        string designation,
        string officeid,
        string officname,
        string deptid,
        string deptname,
        string mobileno,
        string Emailid,
        string designationid,
        string AccNo,
        string ifscCode
    );

    public record HrmsResponseDto(
        int status,
        string message,
        List<EmpDetailDto> Empdetails
    );

    // FleetStrength DTOs
    public record FleetStrengthRequestDto(
        int DeptId,
        int DistrictId,
        int VehicleTypeId,
        int FleetStrength
    );

    public record FleetStrengthResponseDto(
        int FleetStrengthId,
        int DeptId,
        int DistrictId,
        int VehicleTypeId,
        int FleetStrengthValue,
        string DepartmentName,
        string DistrictName,
        string VehicleTypeName,
        DateTime? PDate,
        DateTime? TDate
    );

    // StoreItem (InventoryItem) DTOs
    public record StoreItemRequestDto(
        string Name,
        string? Category,
        string? Description,
        bool IsModelRequired,
        bool IsActive
    );

    public record StoreItemResponseDto(
        int InventoryItemId,
        string Name,
        string? Category,
        string? Description,
        bool IsModelRequired,
        bool IsActive,
        DateTime CreatedAt
    );
}

