using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NGD.OneVerse.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHealthChecks();

// JWT
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? "NGD.OneVerse.AIOS";

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? "NGD.OneVerse.Client";

var jwtSecret = Environment.GetEnvironmentVariable("NGD_JWT_SECRET");

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException(
        "NGD_JWT_SECRET environment variable is not configured.");
}

if (Encoding.UTF8.GetBytes(jwtSecret).Length < 32)
{
    throw new InvalidOperationException(
        "NGD_JWT_SECRET must contain at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// NGD OneVerse service registrations added by service-wiring repair (20261023-200315).
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Organization.Interfaces.IOrganizationService, global::NGD.OneVerse.Application.Modules.Organization.OrganizationService>();
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Site.Interfaces.ISiteService, global::NGD.OneVerse.Application.Modules.Site.SiteService>();
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Employee.Interfaces.IEmployeeService, global::NGD.OneVerse.Application.Modules.Employee.EmployeeService>();
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Employee.Interfaces.IEmployeeRepository, global::NGD.OneVerse.Infrastructure.Persistence.Repositories.EmployeeRepository>();
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Organization.Interfaces.IOrganizationRepository, global::NGD.OneVerse.Infrastructure.Persistence.Repositories.OrganizationRepository>();
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Site.Interfaces.ISiteRepository, global::NGD.OneVerse.Infrastructure.Persistence.Repositories.SiteRepository>();
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Department.Interfaces.IDepartmentService, global::NGD.OneVerse.Application.Modules.Department.DepartmentService>();
builder.Services.AddScoped<global::NGD.OneVerse.Application.Modules.Department.Interfaces.IDepartmentRepository, global::NGD.OneVerse.Infrastructure.Persistence.Repositories.DepartmentRepository>();
builder.Services.AddScoped<global::NGD.OneVerse.Api.Services.PasswordService>();
builder.Services.AddScoped<global::NGD.OneVerse.Api.Services.JwtTokenService>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, global::NGD.OneVerse.Api.Authorization.PermissionPolicyProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, global::NGD.OneVerse.Api.Authorization.PermissionAuthorizationHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    application = "NGD OneVerse AIOS",
    status = "Running",
    version = "1.0",
    timestamp = DateTime.UtcNow
}));

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();


