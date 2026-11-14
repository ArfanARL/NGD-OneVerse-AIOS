using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NGD.OneVerse.Api.Contracts.Auth;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Services;

public sealed class JwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _db;

    public JwtTokenService(
        IConfiguration configuration,
        ApplicationDbContext db)
    {
        _configuration = configuration;
        _db = db;
    }

    public async Task<LoginResponse> CreateTokenAsync(User user)
    {
        var assignments = await _db.Set<UserRole>()
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Role)
                .ThenInclude(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission)
            .ToListAsync();

        var roles = assignments
            .Where(x => x.Role.IsActive)
            .Select(x => x.Role.Code)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var permissions = assignments
            .Where(x => x.Role.IsActive)
            .SelectMany(x => x.Role.RolePermissions)
            .Where(x => x.Permission.IsActive)
            .Select(x => x.Permission.Code)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var section = _configuration.GetSection("Jwt");

        // Keep token creation aligned with Program.cs token validation.
        var issuer = section["Issuer"] ?? "NGD.OneVerse.AIOS";
        var audience = section["Audience"] ?? "NGD.OneVerse.Client";

        // Use one secret source for signing and validation; never store it in appsettings.
        var secret = Environment.GetEnvironmentVariable("NGD_JWT_SECRET");
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "NGD_JWT_SECRET environment variable is not configured.");
        }

        if (Encoding.UTF8.GetBytes(secret).Length < 32)
        {
            throw new InvalidOperationException(
                "NGD_JWT_SECRET must contain at least 32 bytes.");
        }

        var expiresMinutes =
            section.GetValue<int?>("ExpirationMinutes") ?? 60;

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(expiresMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new LoginResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresAtUtc = expires,
            User = new CurrentUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                Roles = roles,
                Permissions = permissions
            }
        };
    }
}

