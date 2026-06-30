using Microsoft.EntityFrameworkCore;
using backend.Models.Core;
using backend.Models.Masters;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        // --- Core Tables ---
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<VehicleInfo> Vehicles { get; set; } 
        public DbSet<VehicleTransfer> VehicleTransfers { get; set; }
        public DbSet<BillClaim> BillClaims { get; set; }
        public DbSet<HiredVehicle> HiredVehicles { get; set; }
        public DbSet<FuelBill> FuelBills { get; set; }
        public DbSet<MaintenanceBill> MaintenanceBills { get; set; }
        public DbSet<HiredVehicleBill> HiredVehicleBills { get; set; }
        public DbSet<ContractualBill> ContractualBills { get; set; }
        public DbSet<MiscellaneousBill> MiscellaneousBills { get; set; }
        public DbSet<TripDetail> TripDetails { get; set; }

        // --- Inventory Tables ---
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<InventoryAllotment> InventoryAllotments { get; set; }

        // --- Master Tables ---
        public DbSet<Department> Departments { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Tehsil> Tehsils { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<PetrolPump> PetrolPumps { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<OfficeType> OfficeTypes { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<FleetStrength> FleetStrengths { get; set; }
        public DbSet<Officer> Officers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<VehicleCondemnation> VehicleCondemnations { get; set; }
        public DbSet<VehicleNOCDetail> VehicleNOCDetails { get; set; }
        public DbSet<ContractualRequisite> ContractualRequisites { get; set; }
        public DbSet<FuelEntry> FuelEntries { get; set; }
        public DbSet<FuelMaintenance> FuelMaintenances { get; set; }
        public DbSet<PersonalUsagePlan> PersonalUsagePlans { get; set; }
        public DbSet<PersonalUsageLog> PersonalUsageLogs { get; set; }

        // Auditing
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<GuestAccessLog> GuestAccessLogs { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<OtpRequest> OtpRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // 1. Configure Department entity with legacy-compatible field names
            modelBuilder.Entity<Department>()
                .HasKey(d => d.DeptId);

            modelBuilder.Entity<Department>()
                .Property(d => d.DeptId)
                .HasColumnName("DeptId");

            modelBuilder.Entity<Department>()
                .Property(d => d.DeptName)
                .HasColumnName("DeptName")
                .IsRequired();

            modelBuilder.Entity<Department>()
                .Property(d => d.DeptAbbre)
                .HasColumnName("DeptAbbre");

            modelBuilder.Entity<Department>()
                .Property(d => d.PDate)
                .HasColumnName("PDate");

            modelBuilder.Entity<Department>()
                .Property(d => d.TDate)
                .HasColumnName("TDate");

            modelBuilder.Entity<Department>()
                .Property(d => d.Enabled)
                .HasColumnName("Enabled");

            // 1. Configure User entity with new field names
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .HasColumnName("UserId");

            modelBuilder.Entity<User>()
                .Property(u => u.EmailId)
                .HasColumnName("EmailId");

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNo)
                .HasColumnName("PhoneNo");

            modelBuilder.Entity<User>()
                .Property(u => u.DeptId)
                .HasColumnName("DeptId");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasColumnName("CreatedDate");

                        modelBuilder.Entity<User>()
                .Property(u => u.Enabled)
                .HasColumnName("Enabled");

            modelBuilder.Entity<User>()
                .Property(u => u.DDORegistrationNo)
                .HasColumnName("DDORegistrationNo");

            // 2. Configure many-to-many relationship between User and Role
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Seed default VMS Roles (The Maker-Checker Hierarchy)
            // 2. Seed default VMS Roles (Static Dates for EF Core!)
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "DDO", Description = "Drawing & Disbursing Officer (Maker)", CreatedAt = seedDate },
                new Role { RoleId = 2, Name = "ADMN", Description = "Administrator (Checker)", CreatedAt = seedDate },
                new Role { RoleId = 3, Name = "NDOF", Description = "Nodal Officer (Verifier)", CreatedAt = seedDate },
                new Role { RoleId = 4, Name = "SEC", Description = "Secretary", CreatedAt = seedDate },
                new Role { RoleId = 5, Name = "HOD", Description = "Head of Department", CreatedAt = seedDate },
                new Role { RoleId = 6, Name = "DCL", Description = "District Collector", CreatedAt = seedDate },
                new Role { RoleId = 7, Name = "FD", Description = "Finance Department", CreatedAt = seedDate },
                new Role { RoleId = 8, Name = "PPOF", Description = "Petrol Pump Officer", CreatedAt = seedDate },
                                new Role { RoleId = 9, Name = "GUEST", Description = "Guest User (OTP Login)", CreatedAt = seedDate },
                new Role { RoleId = 10, Name = "ROFC", Description = "Revenue Officer Level", CreatedAt = seedDate }
            );

            // 2a. Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    UserId = 1, 
                    Username = "admin", 
                    PasswordHash = "$2a$11$v1eUXEH5k565XSNl.0exsOTBEfRQqB8Zzj/6WYFFWNaRLWkBog5PG", // Admin@1234
                    Name = "System Administrator", 
                    DDOCode = "ADMIN", 
                    Enabled = true, 
                    CreatedDate = seedDate,
                    IsGuest = false,
                    IsNonTreasuryDDO = false,
                    FailedAttempts = 0
                }
            );

            // 2b. Seed Admin User Role Mapping (User 1 -> Role 2 [ADMN])
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 2 }
            );

            // 3. Districts and Departments are now managed via Database import (CSV).
            // Hardcoded seed data removed to avoid ID mismatches with legacy data.




            // 6. Configure Project entity with legacy-compatible field names
            modelBuilder.Entity<Project>()
                .HasKey(p => p.ProjectId);

            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectId)
                .HasColumnName("ProjectId");

            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectName)
                .HasColumnName("ProjectName")
                .IsRequired();

            modelBuilder.Entity<Project>()
                .Property(p => p.FuelLitresPerMonth)
                .HasColumnName("FuelLitresPerMonth");

            modelBuilder.Entity<Project>()
                .Property(p => p.MaintenanceAmtPerMonth)
                .HasColumnName("MaintenanceAmtPerMonth");

            modelBuilder.Entity<Project>()
                .Property(p => p.MaintenanceAmtPerAnnum)
                .HasColumnName("MaintenanceAmtPerAnnum");

            modelBuilder.Entity<Project>()
                .Property(p => p.PDate)
                .HasColumnName("PDate");

            modelBuilder.Entity<Project>()
                .Property(p => p.TDate)
                .HasColumnName("TDate");

            modelBuilder.Entity<Project>()
                .Property(p => p.DeptId)
                .HasColumnName("DeptId");

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Department)
                .WithMany()
                .HasForeignKey(p => p.DeptId)
                .OnDelete(DeleteBehavior.Restrict);



            // 3. Configure Designation entity with legacy-compatible field names
            modelBuilder.Entity<Designation>()
                .HasKey(d => d.DesignationId);
            
            modelBuilder.Entity<Designation>()
                .Property(d => d.DesignationId)
                .HasColumnName("DesignationId");
            
            modelBuilder.Entity<Designation>()
                .Property(d => d.DesignationName)
                .HasColumnName("DesignationName")
                .IsRequired();
            
            modelBuilder.Entity<Designation>()
                .Property(d => d.DeptId)
                .HasColumnName("DeptId");
            
            // Legacy date fields (exact names)
            modelBuilder.Entity<Designation>()
                .Property(d => d.PDate)
                .HasColumnName("PDate");
            
            modelBuilder.Entity<Designation>()
                .Property(d => d.TDate)
                .HasColumnName("TDate");
            
            // Fuel limits as int (matching VMS_Backend)
            modelBuilder.Entity<Designation>()
                .Property(d => d.PetrolFuelLimit)
                .HasColumnName("PetrolFuelLimit");
            
            modelBuilder.Entity<Designation>()
                .Property(d => d.DieselFuelLimit)
                .HasColumnName("DieselFuelLimit");
            
            // Maintenance limits as int (matching VMS_Backend)
            modelBuilder.Entity<Designation>()
                .Property(d => d.PetrolMaintenanceLimit)
                .HasColumnName("PetrolMaintenanceLimit");
            
            modelBuilder.Entity<Designation>()
                .Property(d => d.DieselMaintenanceLimit)
                .HasColumnName("DieselMaintenanceLimit");
            
            // Active status - renamed to Enabled
            modelBuilder.Entity<Designation>()
                .Property(d => d.Enabled)
                .HasColumnName("Enabled");
            
            // Configure foreign key relationship with Department
            modelBuilder.Entity<Designation>()
                .HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CONFIGURATION 4: Officer Entity
            // ============================================
            
            // 4. Configure Officer entity with legacy-compatible field names
            modelBuilder.Entity<Officer>()
                .HasKey(o => o.OfficerId);

            modelBuilder.Entity<Officer>()
                .Property(o => o.OfficerId)
                .HasColumnName("OfficerId");

            modelBuilder.Entity<Officer>()
                .Property(o => o.OfficerName)
                .HasColumnName("OfficerName")
                .IsRequired();

            modelBuilder.Entity<Officer>()
                .Property(o => o.OfficerIdString)
                .HasColumnName("OfficerIdString");

            modelBuilder.Entity<Officer>()
                .Property(o => o.HrmsCode)
                .HasColumnName("HrmsCode");

            modelBuilder.Entity<Officer>()
                .Property(o => o.DeptId)
                .HasColumnName("DeptId");

            modelBuilder.Entity<Officer>()
                .Property(o => o.DesignationId)
                .HasColumnName("DesignationId");

            modelBuilder.Entity<Officer>()
                .Property(o => o.DesignationType)
                .HasColumnName("DesignationType");

            modelBuilder.Entity<Officer>()
                .Property(o => o.FuelLimit)
                .HasColumnName("FuelLimit");

            modelBuilder.Entity<Officer>()
                .Property(o => o.FuelLimmitd)
                .HasColumnName("FuelLimmitd");

            modelBuilder.Entity<Officer>()
                .Property(o => o.MaintenanceLimit)
                .HasColumnName("MaintenanceLimit");

            modelBuilder.Entity<Officer>()
                .Property(o => o.MaintenanceLimitd)
                .HasColumnName("MaintenanceLimitd");

            modelBuilder.Entity<Officer>()
                .Property(o => o.Remarks)
                .HasColumnName("Remarks");

            modelBuilder.Entity<Officer>()
                .Property(o => o.FileName)
                .HasColumnName("FileName");

            modelBuilder.Entity<Officer>()
                .Property(o => o.UpdatedBy)
                .HasColumnName("UpdatedBy");

            modelBuilder.Entity<Officer>()
                .Property(o => o.TDate)
                .HasColumnName("TDate");

            modelBuilder.Entity<Officer>()
                .Property(o => o.Enabled)
                .HasColumnName("Enabled");

            // Explicit Foreign Key Mapping
            modelBuilder.Entity<Officer>()
                .HasOne(o => o.Department)
                .WithMany()
                .HasForeignKey(o => o.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Officer>()
                .HasOne(o => o.Designation)
                .WithMany()
                .HasForeignKey(o => o.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

                        modelBuilder.Entity<Office>()
                .HasKey(o => o.OfficeId);

            modelBuilder.Entity<Office>()
                .Property(o => o.OfficeId)
                .HasColumnName("OfficeId")
                .UseIdentityByDefaultColumn();  // Enable auto-increment for PostgreSQL

            modelBuilder.Entity<Office>()
                .HasOne(o => o.Department)
                .WithMany()
                .HasForeignKey(o => o.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Office>()
                .HasOne(o => o.District)
                .WithMany()
                .HasForeignKey(o => o.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Office>()
                .HasOne(o => o.Tehsil)
                .WithMany()
                .HasForeignKey(o => o.TehsilId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<VehicleModel>()
                .HasKey(vm => vm.ModelId);

            modelBuilder.Entity<VehicleModel>()
                .Property(vm => vm.ModelId)
                .HasColumnName("ModelId");

            modelBuilder.Entity<Manufacturer>()
                .HasKey(m => m.ManufacturerId);

            modelBuilder.Entity<Manufacturer>()
                .Property(m => m.ManufacturerId)
                .HasColumnName("ManufacturerId");

            modelBuilder.Entity<VehicleType>()
                .HasKey(vt => vt.VehicleTypeId);

            modelBuilder.Entity<VehicleType>()
                .Property(vt => vt.VehicleTypeId)
                .HasColumnName("VehicleTypeId");

            modelBuilder.Entity<District>()
                .HasKey(d => d.DistrictId);

            modelBuilder.Entity<District>()
                .Property(d => d.DistrictId)
                .HasColumnName("DistrictId");

            modelBuilder.Entity<Tehsil>()
                .HasKey(t => t.TehsilId);

            modelBuilder.Entity<Tehsil>()
                .Property(t => t.TehsilId)
                .HasColumnName("TehsilId");

            modelBuilder.Entity<Designation>()
                .Property(d => d.DesignationId)
                .HasColumnName("DesignationId");

            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectId)
                .HasColumnName("ProjectId");

            modelBuilder.Entity<Allocation>()
                .HasKey(a => a.AllocationTypeId);

            modelBuilder.Entity<Allocation>()
                .Property(a => a.AllocationTypeId)
                .HasColumnName("AllocationTypeId");

            modelBuilder.Entity<OfficeType>()
                .HasKey(ot => ot.OfficeTypeId);

            modelBuilder.Entity<OfficeType>()
                .Property(ot => ot.OfficeTypeId)
                .HasColumnName("OfficeTypeId");

            modelBuilder.Entity<FleetStrength>()
                .HasKey(fs => fs.FleetStrengthId);

            modelBuilder.Entity<FleetStrength>()
                .Property(fs => fs.FleetStrengthId)
                .HasColumnName("FleetStrengthId");

            modelBuilder.Entity<Tehsil>()
                .Property(t => t.DistrictId)
                .HasColumnName("DistrictId");

            modelBuilder.Entity<Tehsil>()
                .Property(t => t.IsActive)
                .HasColumnName("IsActive");

            // Configure foreign key relationship with District
            modelBuilder.Entity<Tehsil>()
                .HasOne(t => t.District)
                .WithMany()
                .HasForeignKey(t => t.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CONFIGURATION 10: VehicleInfo Entity
            // ============================================
            
            // 10. Configure VehicleInfo entity with legacy-compatible field names
            modelBuilder.Entity<VehicleInfo>()
                .HasKey(v => v.VehicleInfoId);

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.VehicleInfoId)
                .HasColumnName("VehicleInfoId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.VehicleNumber)
                .HasColumnName("VehicleNumber")
                .IsRequired();

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.EngineChasisNumber)
                .HasColumnName("EngineChasisNumber");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.DeptId)
                .HasColumnName("DeptId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.OfficeId)
                .HasColumnName("OfficeId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.OfficerId)
                .HasColumnName("OfficerId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.ProjectId)
                .HasColumnName("ProjectId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.DesignationId)
                .HasColumnName("DesignationId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.verificationstatus)
                .HasColumnName("verificationstatus");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.IsVerified)
                .HasColumnName("IsVerified");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.Comments)
                .HasColumnName("Comments");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.Ishaveyoupurchasednewvehicle)
                .HasColumnName("ishaveyoupurchasednewvehicle");
            
            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.VehiclePurchaseDate)
                .HasColumnName("VehiclePurchaseDate");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.FitnessUpto)
                .HasColumnName("FitnessUpto");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.IsTyreOriginal)
                .HasColumnName("IsTyreOriginal");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.LastTyreChangedDate)
                .HasColumnName("LastTyreChangedDate");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.LastTyreChangedKM)
                .HasColumnName("LastTyreChangedKM");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.KM_30062017)
                .HasColumnName("KM_30062017");

            
            
            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.FuelConsumptionCostDate)
                .HasColumnName("FuelConsumptionCostDate");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.FuelConsumptionCost)
                .HasColumnName("FuelConsumptionCost");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.FuelConsumptionLitres)
                .HasColumnName("FuelConsumptionLitres");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.LastThreeYearsMaintenanceCost)
                .HasColumnName("LastThreeYearsMaintenanceCost");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.VehicleCost)
                .HasColumnName("VehicleCost");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.VehiclePurchaseType)
                .HasColumnName("VehiclePurchaseType");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.OtherPurchaseTypeDetails)
                .HasColumnName("OtherPurchaseTypeDetails");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.NewFleetStrength)
                .HasColumnName("NewFleetStrength");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.VehicleSource)
                .HasColumnName("VehicleSource");
            
            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.LastThreeYearsMaintenanceCostDate)
                .HasColumnName("LastThreeYearsMaintenanceCostDate");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.vehicleproofuploads)
                .HasColumnName("vehicleproofuploads");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.RequisitionDeptId)
                .HasColumnName("RequisitionDeptId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.RequisitionOfficeId)
                .HasColumnName("RequisitionOfficeId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.MaintenenceDuration)
                .HasColumnName("MaintenenceDuration");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.ReadingUptodate)
                .HasColumnName("ReadingUptodate");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.FinancialYearReading)
                .HasColumnName("FinancialYearReading");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.temporpermanent)
                .HasColumnName("temporpermanent");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.VehicleNOC)
                .HasColumnName("VehicleNOC");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.NOC_IssueDate)
                .HasColumnName("NOC_IssueDate");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.NodalOfficerName)
                .HasColumnName("NodalOfficerName");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.NodalOfficerMobileNo)
                .HasColumnName("NodalOfficerMobileNo");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.NodalOfficerUsername)
                .HasColumnName("NodalOfficerUsername");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.NodalOfficerEmail)
                .HasColumnName("NodalOfficerEmail");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.DDOId)
                .HasColumnName("DDOId");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.PDate)
                .HasColumnName("PDate");

            modelBuilder.Entity<VehicleInfo>()
                .Property(v => v.TDate)
                .HasColumnName("TDate");

            // Configure foreign key relationships
            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.Department)
                .WithMany()
                .HasForeignKey(v => v.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.Office)
                .WithMany()
                .HasForeignKey(v => v.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.Officer)
                .WithMany()
                .HasForeignKey(v => v.OfficerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.Project)
                .WithMany()
                .HasForeignKey(v => v.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.Designation)
                .WithMany()
                .HasForeignKey(v => v.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.Manufacturer)
                .WithMany()
                .HasForeignKey(v => v.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.Model)
                .WithMany()
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleInfo>()
                .HasOne(v => v.VehicleType)
                .WithMany()
                .HasForeignKey(v => v.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CONFIGURATION 11: HiredVehicle Entity
            // ============================================
            
            // 11. Configure HiredVehicle entity with legacy-compatible field names
            modelBuilder.Entity<HiredVehicle>()
                .HasKey(hv => hv.HiredVehicleId);

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.HiredVehicleId)
                .HasColumnName("HiredVehicleId");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.BillNumber)
                .HasColumnName("BillNumber")
                .IsRequired();

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.BillDate)
                .HasColumnName("BillDate");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.VehicleNumber)
                .HasColumnName("VehicleNumber")
                .IsRequired();

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.OfficeId)
                .HasColumnName("OfficeId");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.VehicleTypeId)
                .HasColumnName("VehicleTypeId");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.ManufacturerId)
                .HasColumnName("ManufacturerId");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.ModelId)
                .HasColumnName("ModelId");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.SeatingCapacity)
                .HasColumnName("SeatingCapacity");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.FuelUsed)
                .HasColumnName("FuelUsed");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.ContractorName)
                .HasColumnName("ContractorName");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.ContractorPhoneNumber)
                .HasColumnName("ContractorPhoneNumber");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.BillDateFrom)
                .HasColumnName("BillDateFrom");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.BillDateTo)
                .HasColumnName("BillDateTo");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.KMCovered)
                .HasColumnName("KMCovered");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.BillAmount)
                .HasColumnName("BillAmount");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.Noofvehicles)
                .HasColumnName("Noofvehicles");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.PDate)
                .HasColumnName("PDate");

            modelBuilder.Entity<HiredVehicle>()
                .Property(hv => hv.TDate)
                .HasColumnName("TDate");

            // Configure foreign key relationships
            modelBuilder.Entity<HiredVehicle>()
                .HasOne(hv => hv.Office)
                .WithMany()
                .HasForeignKey(hv => hv.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HiredVehicle>()
                .HasOne(hv => hv.VehicleType)
                .WithMany()
                .HasForeignKey(hv => hv.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HiredVehicle>()
                .HasOne(hv => hv.Manufacturer)
                .WithMany()
                .HasForeignKey(hv => hv.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HiredVehicle>()
                .HasOne(hv => hv.Model)
                .WithMany()
                .HasForeignKey(hv => hv.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CONFIGURATION 12: ContractualRequisite Entity
            // ============================================
            
            // 12. Configure ContractualRequisite entity with legacy-compatible field names
            modelBuilder.Entity<ContractualRequisite>()
                .HasKey(cr => cr.ContractualRequisiteId);

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.ContractualRequisiteId)
                .HasColumnName("ContractualRequisiteId");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.BillNumber)
                .HasColumnName("BillNumber")
                .IsRequired();

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.BillDate)
                .HasColumnName("BillDate");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.BillDateFrom)
                .HasColumnName("BillDateFrom");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.BillDateTo)
                .HasColumnName("BillDateTo");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.DDOCode)
                .HasColumnName("DDOCode");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.VehicleInfoId)
                .HasColumnName("VehicleInfoId");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.VehicleNumber)
                .HasColumnName("VehicleNumber")
                .IsRequired();

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.BillAmount)
                .HasColumnName("BillAmount");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.IsContractual)
                .HasColumnName("IsContractual");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.PDate)
                .HasColumnName("PDate");

            modelBuilder.Entity<ContractualRequisite>()
                .Property(cr => cr.TDate)
                .HasColumnName("TDate");

            // Configure foreign key relationship with VehicleInfo
            modelBuilder.Entity<ContractualRequisite>()
                .HasOne(cr => cr.Vehicle)
                .WithMany()
                .HasForeignKey(cr => cr.VehicleInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CONFIGURATION 13: FuelEntry Entity
            // ============================================
            
            // 13. Configure FuelEntry entity with legacy-compatible field names
            modelBuilder.Entity<FuelEntry>()
                .HasKey(fe => fe.FuelEntryId);

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.FuelEntryId)
                .HasColumnName("FuelEntryId");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.BillNumber)
                .HasColumnName("BillNumber")
                .IsRequired();

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.BillDate)
                .HasColumnName("BillDate");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.VehicleInfoId)
                .HasColumnName("VehicleInfoId");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.VehicleNumber)
                .HasColumnName("VehicleNumber")
                .IsRequired();

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.OdometerReading)
                .HasColumnName("OdometerReading");
                
            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.FuelConsumptionLitres)
                .HasColumnName("FuelConsumptionLitres");
                
            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.FuelConsumptionCost)
                .HasColumnName("FuelConsumptionCost");
            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.Permission)
                .HasColumnName("Permission");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.Nocfile)
                .HasColumnName("Nocfile");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.NocIssueDate)
                .HasColumnName("NocIssueDate");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.NocExpiryDate)
                .HasColumnName("NocExpiryDate");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.SanctionAuthorityMobileNo)
                .HasColumnName("SanctionAuthorityMobileNo");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.IsPersonalUsed)
                .HasColumnName("IsPersonalUsed");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.PDate)
                .HasColumnName("PDate");

            modelBuilder.Entity<FuelEntry>()
                .Property(fe => fe.TDate)
                .HasColumnName("TDate");

            // Configure foreign key relationship with VehicleInfo
            modelBuilder.Entity<FuelEntry>()
                .HasOne(fe => fe.Vehicle)
                .WithMany()
                .HasForeignKey(fe => fe.VehicleInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CONFIGURATION 14: FuelMaintenance Entity
            // ============================================
            
            // 14. Configure FuelMaintenance entity with legacy-compatible field names
            modelBuilder.Entity<FuelMaintenance>()
                .HasKey(fm => fm.FuelMaintenanceId);

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.FuelMaintenanceId)
                .HasColumnName("FuelMaintenanceId");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.BillNumber)
                .HasColumnName("BillNumber")
                .IsRequired();

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.BillDate)
                .HasColumnName("BillDate");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.VehicleInfoId)
                .HasColumnName("VehicleInfoId");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.VehicleNumber)
                .HasColumnName("VehicleNumber")
                .IsRequired();

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.OdometerReading)
                .HasColumnName("OdometerReading");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.Action)
                .HasColumnName("Action");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.Amount)
                .HasColumnName("Amount");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.Details)
                .HasColumnName("Details");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.MaintenanceType)
                .HasColumnName("MaintenanceType");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.SubVoucherNo)
                .HasColumnName("SubVoucherNo");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.SubVoucherDesc)
                .HasColumnName("SubVoucherDesc");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.ExpenditureDetails)
                .HasColumnName("ExpenditureDetails");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.SanctionOrderNo)
                .HasColumnName("SanctionOrderNo");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.SanctionOrderDate)
                .HasColumnName("SanctionOrderDate");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.SanctionAuthority)
                .HasColumnName("SanctionAuthority");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.FirmName)
                .HasColumnName("FirmName");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.FwdToTreasury)
                .HasColumnName("FwdToTreasury");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.ClaimInfo)
                .HasColumnName("ClaimInfo");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IsProduction)
                .HasColumnName("IsProduction");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IsGrantInAidBill)
                .HasColumnName("IsGrantInAidBill");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.GrantInAidPeriod)
                .HasColumnName("GrantInAidPeriod");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.SanctionedBy)
                .HasColumnName("SanctionedBy");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.FDSanctionLetterNo)
                .HasColumnName("FDSanctionLetterNo");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.DateOfIssue)
                .HasColumnName("DateOfIssue");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.Purpose)
                .HasColumnName("Purpose");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IsBulkBill)
                .HasColumnName("IsBulkBill");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IsSupplementaryBill)
                .HasColumnName("IsSupplementaryBill");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.ParentClaimId)
                .HasColumnName("ParentClaimId");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.SupplementaryAllotmentDone)
                .HasColumnName("SupplementaryAllotmentDone");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IFMSStatus)
                .HasColumnName("IFMSStatus");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IFMSBillNo)
                .HasColumnName("IFMSBillNo");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IsNewIFMS)
                .HasColumnName("IsNewIFMS");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.BillInfoDetail)
                .HasColumnName("BillInfoDetail");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.VMSRefNo)
                .HasColumnName("VMSRefNo");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.BillSubmittedDate)
                .HasColumnName("BillSubmittedDate");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.IncomeTaxAmount)
                .HasColumnName("IncomeTaxAmount");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.ClaimNo)
                .HasColumnName("ClaimNo");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.ClaimResponseId)
                .HasColumnName("ClaimResponseId");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.ClaimVerificationStatus)
                .HasColumnName("ClaimVerificationStatus");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.PDate)
                .HasColumnName("PDate");

            modelBuilder.Entity<FuelMaintenance>()
                .Property(fm => fm.TDate)
                .HasColumnName("TDate");

            // Configure foreign key relationship with VehicleInfo
            modelBuilder.Entity<FuelMaintenance>()
                .HasOne(fm => fm.Vehicle)
                .WithMany()
                .HasForeignKey(fm => fm.VehicleInfoId)
                .OnDelete(DeleteBehavior.Restrict);

                    // Configure VehicleTransfer entity with new primary key
            modelBuilder.Entity<VehicleTransfer>()
                .HasKey(vt => vt.VehicleTransferId);

            modelBuilder.Entity<VehicleTransfer>()
                .Property(vt => vt.VehicleTransferId)
                .HasColumnName("VehicleTransferId");

            // Configure PetrolPump entity with new primary key
            modelBuilder.Entity<PetrolPump>()
                .HasKey(pp => pp.PetrolPumpId);

            modelBuilder.Entity<PetrolPump>()
                .Property(pp => pp.PetrolPumpId)
                .HasColumnName("PetrolPumpId");

            // Configure BillClaim entity with new primary key
            modelBuilder.Entity<BillClaim>()
                .HasKey(bc => bc.BillClaimId);

            modelBuilder.Entity<BillClaim>()
                .Property(bc => bc.BillClaimId)
                .HasColumnName("BillClaimId");

            // Configure ContractualBill entity with new primary key
            modelBuilder.Entity<ContractualBill>()
                .HasKey(cb => cb.ContractualBillId);

            modelBuilder.Entity<ContractualBill>()
                .Property(cb => cb.ContractualBillId)
                .HasColumnName("ContractualBillId");

            // Configure FuelBill entity with new primary key
            modelBuilder.Entity<FuelBill>()
                .HasKey(fb => fb.FuelBillId);

            modelBuilder.Entity<FuelBill>()
                .Property(fb => fb.FuelBillId)
                .HasColumnName("FuelBillId");

            // Configure HiredVehicleBill entity with new primary key
            modelBuilder.Entity<HiredVehicleBill>()
                .HasKey(hvb => hvb.HiredVehicleBillId);

            modelBuilder.Entity<HiredVehicleBill>()
                .Property(hvb => hvb.HiredVehicleBillId)
                .HasColumnName("HiredVehicleBillId");

            // Configure InventoryAllotment entity with new primary key
            modelBuilder.Entity<InventoryAllotment>()
                .HasKey(ia => ia.InventoryAllotmentId);

            modelBuilder.Entity<InventoryAllotment>()
                .Property(ia => ia.InventoryAllotmentId)
                .HasColumnName("InventoryAllotmentId");

            // Configure InventoryItem entity with new primary key
            modelBuilder.Entity<InventoryItem>()
                .HasKey(ii => ii.InventoryItemId);

            modelBuilder.Entity<InventoryItem>()
                .Property(ii => ii.InventoryItemId)
                .HasColumnName("InventoryItemId");

            // Configure MaintenanceBill entity with new primary key
            modelBuilder.Entity<MaintenanceBill>()
                .HasKey(mb => mb.MaintenanceBillId);

            modelBuilder.Entity<MaintenanceBill>()
                .Property(mb => mb.MaintenanceBillId)
                .HasColumnName("MaintenanceBillId");

            // Configure Role entity with new primary key
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleId)
                .HasColumnName("RoleId");

            // Configure StockTransaction entity with new primary key
            modelBuilder.Entity<StockTransaction>()
                .HasKey(st => st.StockTransactionId);

            modelBuilder.Entity<StockTransaction>()
                .Property(st => st.StockTransactionId)
                .HasColumnName("StockTransactionId");

            // Configure VehicleCondemnation entity with new primary key
            modelBuilder.Entity<VehicleCondemnation>()
                .HasKey(vc => vc.VehicleCondemnationId);

            modelBuilder.Entity<VehicleCondemnation>()
                .Property(vc => vc.VehicleCondemnationId)
                .HasColumnName("VehicleCondemnationId");
            // Configure PasswordHistory relationship
            modelBuilder.Entity<PasswordHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PasswordHistories)
                .HasForeignKey(ph => ph.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
