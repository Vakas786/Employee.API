using Asp.Versioning;
using AutoMapper;
using Employee.API.Data;
using Employee.API.Extensions;
using Employee.API.Interfaces;
using Employee.API.Mappings;
using Employee.API.Repository;
using Employee.API.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// Serilog
// ==========================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// ==========================================
// Database
// ==========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// API Versioning
// ==========================================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// ==========================================
// Health Checks
// ==========================================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// ==========================================
// Rate Limiter
// ==========================================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ==========================================
// MVC & Swagger
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCaching();

// ==========================================
// Dependency Injection
// ==========================================
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// ==========================================
// Authorization
// ==========================================
builder.Services.AddAuthorization();

// ==========================================
// Build App
// ==========================================
var app = builder.Build();

// ==========================================
// Middleware
// ==========================================
app.UseSerilogRequestLogging();

app.UseGlobalExceptionMiddleware();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseResponseCaching();

app.UseRateLimiter();

// Authentication is disabled for now
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();