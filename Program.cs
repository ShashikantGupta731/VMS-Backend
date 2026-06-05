using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using backend.Data;
using backend.Services;
using backend.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddOpenApi();

// Configure file upload settings
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5242880; // 5MB
});
builder.Services.Configure<backend.Configurations.SftpSettings>(builder.Configuration.GetSection("SftpSettings"));

// Configure PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder
            .WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<ICaptchaService, CaptchaService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIfmsService, IfmsService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPetrolPumpService, PetrolPumpService>();

// Register Dynamic Report Strategies
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.IncorrectOdometerReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.FuelLimitExceededReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.OfficerMultipleVehiclesReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.DdosNotMappedReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VehicleDetailsReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.CondemnedVehicleReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.FitnessCertificateReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.PublicGuestRecordsReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VehicleMaintenanceReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VehicleFuelInfoReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.DesignationWiseFuelLimitReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VerifiedUnverifiedVehicleReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.AllocationTypeWiseDataReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.TehsilsWithNoVehiclesReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VehicleModelReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.NotPostingUnderPolReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VehicleTransferReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.AllocationWiseBillingReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VoucherTypeBillsReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.VehicleExpenditureReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.DepartmentWiseReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.DistrictWiseReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.OfficeWiseReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.DesignationWiseReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.FinancialYearExpenditureStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategy, backend.Services.Reports.GrnDetailsReportStrategy>();
builder.Services.AddScoped<backend.Services.Reports.IReportStrategyFactory, backend.Services.Reports.ReportStrategyFactory>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["Jwt:SecretKey"];
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// --- SELF-HEALING DATABASE STARTUP CHECKS ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        context.Database.ExecuteSqlRaw("ALTER TABLE \"Vehicles\" ADD COLUMN IF NOT EXISTS \"ishaveyoupurchasednewvehicle\" boolean NULL;");
        Console.WriteLine("[STARTUP] Verified and ensured 'ishaveyoupurchasednewvehicle' column exists in 'Vehicles' table.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[STARTUP] Warning during column self-healing check: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

//app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseStaticFiles(); // Serve static files
app.UseAuthentication();
app.UseMiddleware<ActivityLoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();