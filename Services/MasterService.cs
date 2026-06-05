using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Masters;
using backend.Models.Masters;
using backend.Models.Core;

namespace backend.Services
{
    public class MasterService : IMasterService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MasterService(AppDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Where(d => d.Enabled == true)
                .Select(d => new DepartmentResponseDto(
                    d.DeptId, 
                    d.DeptName, 
                    d.DeptAbbre, 
                    d.Enabled, 
                    d.PDate,
                    d.TDate
                ))
                .ToListAsync();
        }

        public async Task<List<DepartmentResponseDto>> GetDepartmentsByUserAsync(int userId, int? userDeptId)
        {
            // If user has a specific department, return only that department
            if (userDeptId.HasValue && userDeptId.Value > 0)
            {
                var dept = await _context.Departments
                    .Where(d => d.DeptId == userDeptId.Value && d.Enabled == true)
                    .Select(d => new DepartmentResponseDto(
                        d.DeptId, 
                        d.DeptName, 
                        d.DeptAbbre, 
                        d.Enabled, 
                        d.PDate,
                        d.TDate
                    ))
                    .FirstOrDefaultAsync();

                return dept != null ? new List<DepartmentResponseDto> { dept } : new List<DepartmentResponseDto>();
            }

            // Fallback: try to find departments where a user has associated offices
            var deptIds = await _context.Offices
                .Where(o => o.UserId == userId.ToString() && o.Enabled)
                .Select(o => o.DeptId)
                .Distinct()
                .ToListAsync();

            if (deptIds.Any())
            {
                return await _context.Departments
                    .Where(d => deptIds.Contains(d.DeptId) && d.Enabled == true)
                    .Select(d => new DepartmentResponseDto(
                        d.DeptId, 
                        d.DeptName, 
                        d.DeptAbbre, 
                        d.Enabled, 
                        d.PDate,
                        d.TDate
                    ))
                    .ToListAsync();
            }

            // If nothing found, return all departments as fallback
            return await GetAllDepartmentsAsync();
        }

        public async Task<List<DropdownResponseDto>> GetAllDistrictsAsync()
        {
            return await _context.Districts
                .Where(d => d.IsActive)
                .Select(d => new DropdownResponseDto(d.DistrictId, d.DistrictName))
                .ToListAsync();
        }

        public async Task<List<DropdownResponseDto>> GetDistrictsByUserAsync(int? userDistId)
        {
            if (userDistId.HasValue && userDistId.Value > 0)
            {
                var district = await _context.Districts
                    .Where(d => d.DistrictId == userDistId.Value && d.IsActive)
                    .Select(d => new DropdownResponseDto(d.DistrictId, d.DistrictName))
                    .FirstOrDefaultAsync();

                return district != null ? new List<DropdownResponseDto> { district } : new List<DropdownResponseDto>();
            }

            return await GetAllDistrictsAsync();
        }

        public async Task<List<DropdownResponseDto>> GetAllTehsilsAsync(int? districtId = null)
        {
            if (districtId.HasValue && districtId.Value > 0)
            {
                return await _context.Tehsils
                    .Where(t => t.IsActive && t.DistrictId == districtId.Value)
                    .Select(t => new DropdownResponseDto(t.TehsilId, t.TehsilName))
                    .ToListAsync();
            }

            return await _context.Tehsils
                .Where(t => t.IsActive)
                .Select(t => new DropdownResponseDto(t.TehsilId, t.TehsilName))
                .ToListAsync();
        }

        public async Task<List<DropdownResponseDto>> GetAllOfficeTypesAsync()
        {
            return await _context.OfficeTypes
                .Select(t => new DropdownResponseDto(t.OfficeTypeId, t.OfficeTypeName))
                .ToListAsync();
        }

        public async Task<OfficeResponseDto?> GetOfficeByIdAsync(int id)
        {
            return await _context.Offices
                .Include(o => o.Department)
                .Include(o => o.District)
                .Include(o => o.Tehsil)
                .Where(o => o.OfficeId == id && o.Enabled)
                .Select(o => new OfficeResponseDto(
                    o.OfficeId, 
                    o.OfficeName, 
                    o.OfficeAddress, 
                    o.DeptId, 
                    o.Department != null ? o.Department.DeptName : "Unknown Department",
                    o.DistrictId,
                    o.District != null ? o.District.DistrictName : "Unknown District",
                    o.TehsilId,
                    o.Tehsil != null ? o.Tehsil.TehsilName : null,
                    o.OfficeAbbreviation,
                    o.OfficeTypeId,
                    o.OfficeTypeOther,
                    o.UserId,
                    o.Enabled
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<OfficeResponseDto> AddOfficeAsync(OfficeRequestDto request)
        {
            var office = new Office
            {
                OfficeName = request.OfficeName,
                OfficeAddress = request.OfficeAddress,
                DeptId = request.DeptId,
                DistrictId = request.DistrictId,
                TehsilId = request.TehsilId,
                OfficeAbbreviation = request.OfficeAbbreviation,
                OfficeTypeId = request.OfficeTypeId,
                OfficeTypeOther = request.OfficeTypeOther,
                UserId = request.UserId,
                PDate = DateTime.UtcNow,
                Enabled = true
            };

            _context.Offices.Add(office);
            await _context.SaveChangesAsync();

            return await GetOfficeByIdAsync(office.OfficeId) 
                   ?? throw new Exception("Error retrieving newly created office");
        }

        public async Task<OfficeResponseDto?> UpdateOfficeAsync(int id, OfficeRequestDto request)
        {
            var office = await _context.Offices.FindAsync(id);
            if (office == null || !office.Enabled) return null;

            office.OfficeName = request.OfficeName;
            office.OfficeAddress = request.OfficeAddress;
            office.DeptId = request.DeptId;
            office.TehsilId = request.TehsilId;
            office.OfficeAbbreviation = request.OfficeAbbreviation;
            office.OfficeTypeId = request.OfficeTypeId;
            office.OfficeTypeOther = request.OfficeTypeOther;
            office.TDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetOfficeByIdAsync(id);
        }

        public async Task<bool> DeleteOfficeAsync(int id)
        {
            var office = await _context.Offices.FindAsync(id);
            if (office == null) return false;

            office.Enabled = false;
            office.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OfficeResponseDto>> GetAllOfficesAsync(int? departmentId = null)
        {
            var query = _context.Offices
                .Include(o => o.Department)
                .Include(o => o.District)
                .Include(o => o.Tehsil)
                .Where(o => o.Enabled).AsQueryable();

            if (departmentId.HasValue)
            {
                query = query.Where(o => o.DeptId == departmentId.Value);
            }

            return await query
                .Select(o => new OfficeResponseDto(
                    o.OfficeId, 
                    o.OfficeName, 
                    o.OfficeAddress, 
                    o.DeptId, 
                    o.Department != null ? o.Department.DeptName : "Unknown Department",
                    o.DistrictId,
                    o.District != null ? o.District.DistrictName : "Unknown District",
                    o.TehsilId,
                    o.Tehsil != null ? o.Tehsil.TehsilName : null,
                    o.OfficeAbbreviation,
                    o.OfficeTypeId,
                    o.OfficeTypeOther,
                    o.UserId,
                    o.Enabled
                ))
                .ToListAsync();
        }

        public async Task<List<OfficeResponseDto>> GetOfficesByUserContextAsync(
            int userId, 
            string userRole, 
            int? userDepartmentId = null, 
            int? userDistrictId = null)
        {
            var query = _context.Offices
                .Include(o => o.Department)
                .Include(o => o.District)
                .Include(o => o.Tehsil)
                .Where(o => o.Enabled).AsQueryable();

            // Role-based filtering logic
            switch (userRole)
            {
                // Administrators see everything
                case "ADMN":
                case "FD":
                    break; // No filtering - they see all

                // HOD sees offices in their department AND only their specific offices
                case "HOD":
                    if (userDepartmentId.HasValue)
                    {
                        query = query.Where(o => o.DeptId == userDepartmentId.Value);
                    }
                    query = query.Where(o => o.UserId == userId.ToString());
                    break;

                // DDO sees only their specific offices (Ownership logic from legacy)
                case "DDO":
                    query = query.Where(o => o.UserId == userId.ToString());
                    break;

                // DCL sees all offices in their district
                case "DCL":
                    if (userDistrictId.HasValue)
                    {
                        query = query.Where(o => o.DistrictId == userDistrictId.Value);
                    }
                    break;

                // SEC also follows ownership logic
                case "SEC":
                    query = query.Where(o => o.UserId == userId.ToString());
                    break;

                // Nodal/PPO/Revenue Officers see offices based on level
                case "NDOF":

                case "PPOF":
                case "ROFC":
                    // First try district, then department
                    if (userDistrictId.HasValue && userDistrictId.Value > 0)
                    {
                        query = query.Where(o => o.DistrictId == userDistrictId.Value);
                    }
                    else if (userDepartmentId.HasValue && userDepartmentId.Value > 0)
                    {
                        query = query.Where(o => o.DeptId == userDepartmentId.Value);
                    }
                    break;

                // For unknown roles, restrict to their department/district if available
                default:
                    if (userDepartmentId.HasValue)
                    {
                        query = query.Where(o => o.DeptId == userDepartmentId.Value);
                    }
                    else if (userDistrictId.HasValue)
                    {
                        query = query.Where(o => o.DistrictId == userDistrictId.Value);
                    }
                    break;
            }

            return await query
                .Select(o => new OfficeResponseDto(
                    o.OfficeId, 
                    o.OfficeName, 
                    o.OfficeAddress, 
                    o.DeptId, 
                    o.Department != null ? o.Department.DeptName : "Unknown Department",
                    o.DistrictId,
                    o.District != null ? o.District.DistrictName : "Unknown District",
                    o.TehsilId,
                    o.Tehsil != null ? o.Tehsil.TehsilName : null,
                    o.OfficeAbbreviation,
                    o.OfficeTypeId,
                    o.OfficeTypeOther,
                    o.UserId,
                    o.Enabled
                ))
                .ToListAsync();
        }

        public async Task<List<DesignationResponseDto>> GetAllDesignationsAsync(int? officeId = null)
        {
            var query = _context.Designations
                .Include(d => d.Department)
                .Where(d => d.Enabled).AsQueryable();

            if (officeId.HasValue)
            {
                var office = await _context.Offices.FindAsync(officeId.Value);
                if (office != null)
                {
                    query = query.Where(d => d.DeptId == office.DeptId);
                }
            }

            return await query
                .Select(d => new DesignationResponseDto(
                    d.DesignationId, 
                    d.DesignationName, 
                    d.Department.DeptName,
                    d.DesignationType,
                    d.PetrolFuelLimit,
                    d.DieselFuelLimit,
                    d.PetrolMaintenanceLimit,
                    d.DieselMaintenanceLimit
                ))
                .ToListAsync();
        }

        // NEW: Fetch designations directly by department ID (matching legacy GetDesignationByDeptId)
        public async Task<List<DesignationResponseDto>> GetDesignationsByDepartmentAsync(int departmentId)
        {
            return await _context.Designations
                .Include(d => d.Department)
                .Where(d => d.Enabled && d.DeptId == departmentId)
                .Select(d => new DesignationResponseDto(
                    d.DesignationId, 
                    d.DesignationName, 
                    d.Department.DeptName,
                    d.DesignationType,
                    d.PetrolFuelLimit,
                    d.DieselFuelLimit,
                    d.PetrolMaintenanceLimit,
                    d.DieselMaintenanceLimit
                ))
                .ToListAsync();
        }

        public async Task<List<DropdownResponseDto>> GetAllManufacturersAsync()
        {
            return await _context.Manufacturers
                .Where(m => m.IsActive)
                .Select(m => new DropdownResponseDto(m.ManufacturerId, m.ManufacturerName))
                .ToListAsync();
        }

        public async Task<List<DropdownResponseDto>> GetAllVehicleTypesAsync()
        {
            return await _context.VehicleTypes
                .Where(v => v.IsActive)
                .Select(v => new DropdownResponseDto(v.VehicleTypeId, v.VehicleTypeName))
                .ToListAsync();
        }

        public async Task<List<DropdownResponseDto>> GetVehicleModelsByManufacturerAsync(int manufacturerId)
        {
            return await _context.VehicleModels
                .Where(m => m.IsActive && m.ManufacturerId == manufacturerId)
                .Select(m => new DropdownResponseDto(m.ModelId, m.ModelName))
                .ToListAsync();
        }

        public async Task<List<VehicleModelResponseDto>> GetAllVehicleModelsAsync()
        {
            return await _context.VehicleModels
                .Include(m => m.Manufacturer)
                .Include(m => m.VehicleType)
                .Select(m => new VehicleModelResponseDto(
                    m.ModelId, 
                    m.ModelName, 
                    m.Manufacturer.ManufacturerName, 
                    m.SeatingCapacity, 
                    m.VehicleType.VehicleTypeName
                ))
                .ToListAsync();
        }

        public async Task<List<OfficerResponseDto>> GetAllOfficersAsync()
        {
            return await _context.Officers
                .AsNoTracking()
                .Include(o => o.Designation)
                                .Where(o => o.Enabled)
                .Select(o => new OfficerResponseDto(
                    o.OfficerId,
                    o.OfficerName.Split(' ').FirstOrDefault() ?? "", // Extract FirstName from OfficerName
                    o.OfficerName.Split(' ').Skip(1).FirstOrDefault() ?? "", // Extract LastName from OfficerName
                    o.OfficerName,
                    o.HrmsCode,
                    o.DesignationId,
                    o.Designation != null ? o.Designation.DesignationName : null,  
                    null, // OfficeId - not available in new model
                    null, // Office Name - not available in new model
                    o.FuelLimit,
                    o.FuelLimmitd,
                    o.MaintenanceLimit,
                    o.MaintenanceLimitd,
                    o.Remarks
                ))
                .ToListAsync();
        }

        public async Task<OfficerResponseDto?> GetOfficerByIdAsync(int id)
        {
            return await _context.Officers
                .AsNoTracking()
                .Include(o => o.Designation)
                                .Where(o => o.OfficerId == id && o.Enabled)
                .Select(o => new OfficerResponseDto(
                    o.OfficerId,
                    o.OfficerName.Split(' ').FirstOrDefault() ?? "", // Extract FirstName from OfficerName
                    o.OfficerName.Split(' ').Skip(1).FirstOrDefault() ?? "", // Extract LastName from OfficerName
                    o.OfficerName,
                    o.HrmsCode,
                    o.DesignationId,
                    o.Designation != null ? o.Designation.DesignationName : null,
                    null, // OfficeId - not available in new model
                    null, // Office Name - not available in new model
                    o.FuelLimit,
                    o.FuelLimmitd,
                    o.MaintenanceLimit,
                    o.MaintenanceLimitd,
                    o.Remarks
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<OfficerResponseDto> AddOfficerAsync(OfficerRequestDto request)
        {
            var officer = new Officer
            {
                OfficerIdString = request.FirstName, // Using FirstName as OfficerIdString (legacy compatibility)
                OfficerName = $"{request.FirstName} {request.LastName}",
                HrmsCode = request.HRMSCode,
                DeptId = request.DesignationId ?? 0, // Using DesignationId as DeptId temporarily
                DesignationId = request.DesignationId ?? 0,
                DesignationType = 1, // Default value
                FuelLimit = (int)request.PetrolFuelLimit,
                FuelLimmitd = (int)request.DieselFuelLimit,
                MaintenanceLimit = (int)request.PetrolMaintenanceLimit,
                MaintenanceLimitd = (int)request.DieselMaintenanceLimit,
                Remarks = request.Remarks ?? string.Empty,
                FileName = "",
                UpdatedBy = "System",
                Enabled = true
            };

            _context.Officers.Add(officer);
            await _context.SaveChangesAsync();

            return await GetOfficerByIdAsync(officer.OfficerId) ?? throw new Exception("Error retrieving newly created officer");
        }

        public async Task<OfficerResponseDto?> UpdateOfficerAsync(int id, OfficerRequestDto request)
        {
            var officer = await _context.Officers.FindAsync(id);
            if (officer == null || !officer.Enabled) return null;

            officer.OfficerIdString = request.FirstName; // Using FirstName as OfficerIdString (legacy compatibility)
            officer.OfficerName = $"{request.FirstName} {request.LastName}";
            officer.HrmsCode = request.HRMSCode;
            officer.DeptId = request.DesignationId ?? 0; // Using DesignationId as DeptId temporarily
            officer.DesignationId = request.DesignationId ?? 0;
            officer.FuelLimit = (int)request.PetrolFuelLimit;
            officer.FuelLimmitd = (int)request.DieselFuelLimit;
            officer.MaintenanceLimit = (int)request.PetrolMaintenanceLimit;
            officer.MaintenanceLimitd = (int)request.DieselMaintenanceLimit;
            officer.Remarks = request.Remarks ?? string.Empty;

            await _context.SaveChangesAsync();
            return await GetOfficerByIdAsync(id);
        }

        public async Task<bool> DeleteOfficerAsync(int id)
        {
            var officer = await _context.Officers.FindAsync(id);
            if (officer == null) return false;

            officer.Enabled = false; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProjectResponseDto>> GetAllProjectsAsync(int? departmentId = null)
        {
            var query = _context.Projects.AsNoTracking().Where(p => true);
            
            if (departmentId.HasValue)
            {
                query = query.Where(p => p.DeptId == departmentId.Value);
            }

            return await query
                .Select(p => new ProjectResponseDto(
                    p.ProjectId, 
                    p.ProjectName, 
                    p.DeptId,
                    (decimal)p.MaintenanceAmtPerMonth,
                    (decimal)p.MaintenanceAmtPerAnnum
                ))
                .ToListAsync();
        }

        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.ProjectId == id && true)
                .Select(p => new ProjectResponseDto(
                    p.ProjectId, 
                    p.ProjectName, 
                    p.DeptId,
                    (decimal)p.MaintenanceAmtPerMonth,
                    (decimal)p.MaintenanceAmtPerAnnum
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectResponseDto> AddProjectAsync(ProjectRequestDto request)
        {
            var project = new Project
            {
                ProjectName = request.Name,
                DeptId = request.DepartmentId,
                MaintenanceAmtPerMonth = (double)request.PetrolFuelLimit,
                MaintenanceAmtPerAnnum = (double)request.PetrolMaintenanceLimit,
                PDate = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return await GetProjectByIdAsync(project.ProjectId) ?? throw new Exception("Error retrieving newly created project");
        }

        public async Task<ProjectResponseDto?> UpdateProjectAsync(int id, ProjectRequestDto request)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return null;

            project.ProjectName = request.Name;
            project.DeptId = request.DepartmentId;
            project.MaintenanceAmtPerMonth = (double)request.PetrolFuelLimit;
            project.MaintenanceAmtPerAnnum = (double)request.PetrolMaintenanceLimit;

            await _context.SaveChangesAsync();
            return await GetProjectByIdAsync(id);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- District CRUD ---
        public async Task<DropdownResponseDto> AddDistrictAsync(DistrictRequestDto request)
        {
            var district = new District
            {
                DistrictName = request.DistrictName,
                DistAbbre = request.DistAbbre,
                PDate = DateTime.UtcNow,
                IsActive = request.IsActive
            };
            _context.Districts.Add(district);
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(district.DistrictId, district.DistrictName);
        }

        public async Task<DropdownResponseDto?> UpdateDistrictAsync(int id, DistrictRequestDto request)
        {
            var district = await _context.Districts.FindAsync(id);
            if (district == null) return null;
            
            district.DistrictName = request.DistrictName;
            district.DistAbbre = request.DistAbbre;
            district.TDate = DateTime.UtcNow;
            district.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
            
            return new DropdownResponseDto(district.DistrictId, district.DistrictName);
        }

        public async Task<bool> DeleteDistrictAsync(int id)
        {
            var district = await _context.Districts.FindAsync(id);
            if (district == null) return false;
            
            // Soft delete
            district.IsActive = false;
            district.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Tehsil CRUD ---
        public async Task<DropdownResponseDto> AddTehsilAsync(TehsilRequestDto request)
        {
            var tehsil = new Tehsil
            {
                TehsilName = request.TehsilName,
                DistrictId = request.DistrictId,
                PDate = DateTime.UtcNow,
                IsActive = request.IsActive
            };
            _context.Tehsils.Add(tehsil);
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(tehsil.TehsilId, tehsil.TehsilName);
        }

        public async Task<DropdownResponseDto?> UpdateTehsilAsync(int id, TehsilRequestDto request)
        {
            var tehsil = await _context.Tehsils.FindAsync(id);
            if (tehsil == null) return null;

            tehsil.TehsilName = request.TehsilName;
            tehsil.DistrictId = request.DistrictId;
            tehsil.TDate = DateTime.UtcNow;
            tehsil.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(tehsil.TehsilId, tehsil.TehsilName);
        }

        public async Task<bool> DeleteTehsilAsync(int id)
        {
            var tehsil = await _context.Tehsils.FindAsync(id);
            if (tehsil == null) return false;

            tehsil.IsActive = false;
            tehsil.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // --- VehicleType CRUD ---
        public async Task<DropdownResponseDto> AddVehicleTypeAsync(VehicleTypeRequestDto request)
        {
            var vt = new VehicleType
            {
                VehicleTypeName = request.VehicleTypeName,
                VehicleTypeExample = request.VehicleTypeExample,
                VehicleLifeKM = request.VehicleLifeKM,
                VehicleLifeYears = request.VehicleLifeYears,
                PDate = DateTime.UtcNow,
                IsActive = request.IsActive
            };
            _context.VehicleTypes.Add(vt);
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(vt.VehicleTypeId, vt.VehicleTypeName);
        }

        public async Task<DropdownResponseDto?> UpdateVehicleTypeAsync(int id, VehicleTypeRequestDto request)
        {
            var vt = await _context.VehicleTypes.FindAsync(id);
            if (vt == null) return null;

            vt.VehicleTypeName = request.VehicleTypeName;
            vt.VehicleTypeExample = request.VehicleTypeExample;
            vt.VehicleLifeKM = request.VehicleLifeKM;
            vt.VehicleLifeYears = request.VehicleLifeYears;
            vt.TDate = DateTime.UtcNow;
            vt.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(vt.VehicleTypeId, vt.VehicleTypeName);
        }

        public async Task<bool> DeleteVehicleTypeAsync(int id)
        {
            var vt = await _context.VehicleTypes.FindAsync(id);
            if (vt == null) return false;

            vt.IsActive = false;
            vt.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Manufacturer CRUD ---
        public async Task<DropdownResponseDto> AddManufacturerAsync(ManufacturerRequestDto request)
        {
            var m = new Manufacturer
            {
                ManufacturerName = request.ManufacturerName,
                PDate = DateTime.UtcNow,
                IsActive = request.IsActive
            };
            _context.Manufacturers.Add(m);
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(m.ManufacturerId, m.ManufacturerName);
        }

        public async Task<DropdownResponseDto?> UpdateManufacturerAsync(int id, ManufacturerRequestDto request)
        {
            var m = await _context.Manufacturers.FindAsync(id);
            if (m == null) return null;

            m.ManufacturerName = request.ManufacturerName;
            m.TDate = DateTime.UtcNow;
            m.IsActive = request.IsActive;
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(m.ManufacturerId, m.ManufacturerName);
        }

        public async Task<bool> DeleteManufacturerAsync(int id)
        {
            var m = await _context.Manufacturers.FindAsync(id);
            if (m == null) return false;

            m.IsActive = false;
            m.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // --- OfficeType CRUD ---
        public async Task<DropdownResponseDto> AddOfficeTypeAsync(OfficeTypeRequestDto request)
        {
            var ot = new OfficeType
            {
                OfficeTypeName = request.OfficeTypeName,
                PDate = DateTime.UtcNow
            };
            _context.OfficeTypes.Add(ot);
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(ot.OfficeTypeId, ot.OfficeTypeName);
        }

        public async Task<DropdownResponseDto?> UpdateOfficeTypeAsync(int id, OfficeTypeRequestDto request)
        {
            var ot = await _context.OfficeTypes.FindAsync(id);
            if (ot == null) return null;

            ot.OfficeTypeName = request.OfficeTypeName;
            ot.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(ot.OfficeTypeId, ot.OfficeTypeName);
        }

        public async Task<bool> DeleteOfficeTypeAsync(int id)
        {
            var ot = await _context.OfficeTypes.FindAsync(id);
            if (ot == null) return false;

            _context.OfficeTypes.Remove(ot); // Hard delete as no IsActive
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Allocation CRUD ---
        public async Task<DropdownResponseDto> AddAllocationAsync(AllocationRequestDto request)
        {
            var alloc = new Allocation
            {
                AllocationTypeName = request.AllocationTypeName,
                PDate = DateTime.UtcNow
            };
            _context.Allocations.Add(alloc);
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(alloc.AllocationTypeId, alloc.AllocationTypeName);
        }

        public async Task<DropdownResponseDto?> UpdateAllocationAsync(int id, AllocationRequestDto request)
        {
            var alloc = await _context.Allocations.FindAsync(id);
            if (alloc == null) return null;

            alloc.AllocationTypeName = request.AllocationTypeName;
            alloc.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new DropdownResponseDto(alloc.AllocationTypeId, alloc.AllocationTypeName);
        }

        public async Task<bool> DeleteAllocationAsync(int id)
        {
            var alloc = await _context.Allocations.FindAsync(id);
            if (alloc == null) return false;

            _context.Allocations.Remove(alloc); // Hard delete
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Department CRUD ---
        public async Task<DepartmentResponseDto> AddDepartmentAsync(CreateDepartmentDto request)
        {
            var dept = new Department
            {
                DeptName = request.DepartmentName,
                DeptAbbre = request.DepartmentCode,
                Enabled = request.Enabled,
                PDate = DateTime.UtcNow
            };
            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();
            return new DepartmentResponseDto(dept.DeptId, dept.DeptName, dept.DeptAbbre, dept.Enabled, dept.PDate, dept.TDate);
        }

        public async Task<DepartmentResponseDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto request)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return null;

            dept.DeptName = request.DepartmentName;
            dept.DeptAbbre = request.DepartmentCode;
            dept.Enabled = request.Enabled;
            dept.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return new DepartmentResponseDto(dept.DeptId, dept.DeptName, dept.DeptAbbre, dept.Enabled, dept.PDate, dept.TDate);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return false;

            dept.Enabled = false; // Soft delete
            dept.TDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Models
        public async Task<VehicleModelResponseDto> AddVehicleModelAsync(VehicleModelRequestDto request)
        {
            var model = new VehicleModel
            {
                ModelName = request.ModelName,
                ManufacturerId = request.ManufacturerId,
                SeatingCapacity = request.SeatingCapacity,
                VehicleTypeId = request.VehicleTypeId,
                IsActive = true
            };

            _context.VehicleModels.Add(model);
            await _context.SaveChangesAsync();

            return (await GetAllVehicleModelsAsync()).First(m => m.Id == model.ModelId);
        }

        public async Task<VehicleModelResponseDto?> UpdateVehicleModelAsync(int id, VehicleModelRequestDto request)
        {
            var model = await _context.VehicleModels.FindAsync(id);
            if (model == null) return null;

            model.ModelName = request.ModelName;
            model.ManufacturerId = request.ManufacturerId;
            model.SeatingCapacity = request.SeatingCapacity;
            model.VehicleTypeId = request.VehicleTypeId;

            await _context.SaveChangesAsync();
            return (await GetAllVehicleModelsAsync()).First(m => m.Id == id);
        }

        public async Task<bool> DeleteVehicleModelAsync(int id)
        {
            var model = await _context.VehicleModels.FindAsync(id);
            if (model == null) return false;
            model.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Designations
        public async Task<DesignationResponseDto> AddDesignationAsync(DesignationRequestDto request)
        {
            var desig = new Designation
            {
                DesignationName = request.Name,
                DeptId = request.DepartmentId,
                DesignationType = request.DesignationType,
                PetrolFuelLimit = request.PetrolFuelLimit,
                DieselFuelLimit = request.DieselFuelLimit,
                PetrolMaintenanceLimit = request.PetrolMaintenanceLimit,
                DieselMaintenanceLimit = request.DieselMaintenanceLimit,
                Enabled = true
            };

            _context.Designations.Add(desig);
            await _context.SaveChangesAsync();
            return (await GetAllDesignationsAsync()).First(d => d.DesignationId == desig.DesignationId);
        }

        public async Task<DesignationResponseDto?> UpdateDesignationAsync(int id, DesignationRequestDto request)
        {
            var desig = await _context.Designations.FindAsync(id);
            if (desig == null) return null;

            desig.DesignationName = request.Name;
            desig.DeptId = request.DepartmentId;
            desig.DesignationType = request.DesignationType;
            desig.PetrolFuelLimit = request.PetrolFuelLimit;
            desig.DieselFuelLimit = request.DieselFuelLimit;
            desig.PetrolMaintenanceLimit = request.PetrolMaintenanceLimit;
            desig.DieselMaintenanceLimit = request.DieselMaintenanceLimit;

            await _context.SaveChangesAsync();
            return (await GetAllDesignationsAsync()).First(d => d.DesignationId == id);
        }

        public async Task<bool> DeleteDesignationAsync(int id)
        {
            var desig = await _context.Designations.FindAsync(id);
            if (desig == null) return false;
            desig.Enabled = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // FleetStrength CRUD Operations
        public async Task<List<FleetStrengthResponseDto>> GetAllFleetStrengthsAsync(int? deptId = null, int? districtId = null, int? vehicleTypeId = null)
        {
            var query = _context.FleetStrengths.AsNoTracking();

            if (deptId.HasValue)
            {
                query = query.Where(fs => fs.DeptId == deptId.Value);
            }

            if (districtId.HasValue)
            {
                query = query.Where(fs => fs.DistrictId == districtId.Value);
            }

            if (vehicleTypeId.HasValue)
            {
                query = query.Where(fs => fs.VehicleTypeId == vehicleTypeId.Value);
            }

            return await query
                .Select(fs => new FleetStrengthResponseDto(
                    fs.FleetStrengthId,
                    fs.DeptId,
                    fs.DistrictId,
                    fs.VehicleTypeId,
                    fs.FleetStrengthValue,
                    fs.Department != null ? fs.Department.DeptName : string.Empty,
                    fs.District != null ? fs.District.DistrictName : string.Empty,
                    fs.VehicleType != null ? fs.VehicleType.VehicleTypeName : string.Empty,
                    fs.PDate,
                    fs.TDate
                ))
                .ToListAsync();
        }

        public async Task<FleetStrengthResponseDto?> GetFleetStrengthByIdAsync(int id)
        {
            return await _context.FleetStrengths
                .AsNoTracking()
                .Where(fs => fs.FleetStrengthId == id)
                .Select(fs => new FleetStrengthResponseDto(
                    fs.FleetStrengthId,
                    fs.DeptId,
                    fs.DistrictId,
                    fs.VehicleTypeId,
                    fs.FleetStrengthValue,
                    fs.Department != null ? fs.Department.DeptName : string.Empty,
                    fs.District != null ? fs.District.DistrictName : string.Empty,
                    fs.VehicleType != null ? fs.VehicleType.VehicleTypeName : string.Empty,
                    fs.PDate,
                    fs.TDate
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<FleetStrengthResponseDto> AddFleetStrengthAsync(FleetStrengthRequestDto request)
        {
            var fleetStrength = new FleetStrength
            {
                DeptId = request.DeptId,
                DistrictId = request.DistrictId,
                VehicleTypeId = request.VehicleTypeId,
                FleetStrengthValue = request.FleetStrength,
                PDate = DateTime.UtcNow
            };

            _context.FleetStrengths.Add(fleetStrength);
            await _context.SaveChangesAsync();

            return await GetFleetStrengthByIdAsync(fleetStrength.FleetStrengthId) 
                   ?? throw new Exception("Error retrieving newly created fleet strength");
        }

        public async Task<FleetStrengthResponseDto?> UpdateFleetStrengthAsync(int id, FleetStrengthRequestDto request)
        {
            var fleetStrength = await _context.FleetStrengths.FindAsync(id);
            if (fleetStrength == null) return null;

            fleetStrength.DeptId = request.DeptId;
            fleetStrength.DistrictId = request.DistrictId;
            fleetStrength.VehicleTypeId = request.VehicleTypeId;
            fleetStrength.FleetStrengthValue = request.FleetStrength;

            await _context.SaveChangesAsync();
            return await GetFleetStrengthByIdAsync(id);
        }

        public async Task<bool> DeleteFleetStrengthAsync(int id)
        {
            var fleetStrength = await _context.FleetStrengths.FindAsync(id);
            if (fleetStrength == null) return false;

            _context.FleetStrengths.Remove(fleetStrength);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<HrmsResponseDto?> VerifyHRMSCodeAsync(string hrmsCode)
        {
            var client = _httpClientFactory.CreateClient();
            var hrmsUrl = "https://hrms.punjab.gov.in/api/mobileAPI/EmployeeDetails?Empcd=";
            var path = hrmsUrl + hrmsCode;

            // Using the legacy authorization header as per legacy code analysis
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic bmF2aW5kZXIuc2hhcm1hQG5pYy5pbjo0N0NGMkYzQTgxOTk0MEU3QUExOUM3NDk5NzBCM0M1RjUzNDJFNzNFQ0IwRjQzQzE4Q0MyQkI0M0I5QUE4NTUy");

            try
            {
                var response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<HrmsResponseDto>(content, new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                }
            }
            catch (Exception ex)
            {
                // Log exception if logging is configured
                Console.WriteLine($"HRMS Verification Error: {ex.Message}");
            }

            return null;
        }
        public async Task<List<InventoryDropdownDto>> GetAllInventoryItemsAsync()
        {
            return await _context.InventoryItems
                .Where(i => i.IsActive)
                .Select(i => new InventoryDropdownDto(i.InventoryItemId, i.Name, i.IsModelRequired)).ToListAsync();
        }


        // StoreItems (InventoryItem)
        public async Task<List<StoreItemResponseDto>> GetAllStoreItemsAsync()
        {
            return await _context.InventoryItems.Select(i => new StoreItemResponseDto(i.InventoryItemId, i.Name, i.Category, i.Description, i.IsModelRequired, i.IsActive, i.CreatedAt)).ToListAsync();
        }

        public async Task<StoreItemResponseDto?> GetStoreItemByIdAsync(int id)
        {
            return await _context.InventoryItems.Where(i => i.InventoryItemId == id).Select(i => new StoreItemResponseDto(i.InventoryItemId, i.Name, i.Category, i.Description, i.IsModelRequired, i.IsActive, i.CreatedAt)).FirstOrDefaultAsync();
        }

        public async Task<StoreItemResponseDto> AddStoreItemAsync(StoreItemRequestDto request)
        {
            var entity = new InventoryItem
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                IsModelRequired = request.IsModelRequired,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.InventoryItems.Add(entity);
            await _context.SaveChangesAsync();
            return await GetStoreItemByIdAsync(entity.InventoryItemId) ?? throw new Exception("Error saving StoreItem");
        }

        public async Task<StoreItemResponseDto?> UpdateStoreItemAsync(int id, StoreItemRequestDto request)
        {
            var entity = await _context.InventoryItems.FindAsync(id);
            if (entity == null) return null;

            entity.Name = request.Name;
            entity.Category = request.Category;
            entity.Description = request.Description;
            entity.IsModelRequired = request.IsModelRequired;
            entity.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
            return await GetStoreItemByIdAsync(id);
        }

        public async Task<bool> DeleteStoreItemAsync(int id)
        {
            var entity = await _context.InventoryItems.FindAsync(id);
            if (entity == null) return false;

            _context.InventoryItems.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
