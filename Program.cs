using Asp.Versioning;
using AutoMapper;
using Azure.Identity;
using Employee.API.Data;
using Employee.API.Extensions;
using Employee.API.Interfaces;
using Employee.API.Mappings;
using Employee.API.Repository;
using Employee.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
//var keyVaultUrl = new Uri("https://vakas2026.vault.azure.net/");
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString =
        builder.Configuration["ApplicationInsights:ConnectionString"];
});

//builder.Configuration.AddAzureKeyVault(
//    keyVaultUrl,
//    new DefaultAzureCredential());
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);

    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ReportApiVersions = true;

    options.ApiVersionReader =
        new UrlSegmentApiVersionReader();
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();
builder.Host.UseSerilog();
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
builder.Services.AddControllers();
builder.Services.AddResponseCaching();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// ==========================================
// JWT Authentication
// ==========================================

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;

//    options.SaveToken = true;

//    //options.TokenValidationParameters = new TokenValidationParameters
//    //{
//    //    ValidateIssuer = true,

//    //    ValidateAudience = true,

//    //    ValidateLifetime = true,

//    //    ValidateIssuerSigningKey = true,

//    //    ValidIssuer = builder.Configuration["Jwt:Issuer"],

//    //    ValidAudience = builder.Configuration["Jwt:Audience"],

//    //    IssuerSigningKey = new SymmetricSecurityKey(
//    //        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
//    //};
//});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseGlobalExceptionMiddleware();

app.UseSwagger();

app.UseSwaggerUI();

app.UseAuthentication();

app.UseAuthorization();
app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();