using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Api.Contracts.Auth;
using NGD.OneVerse.Api.Services;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordService _passwordService;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(
        ApplicationDbContext db,
        PasswordService passwordService,
        JwtTokenService jwtTokenService)
    {
        _db = db;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Username and password are required."
            });
        }

        var username = request.Username.Trim();

        var user = await _db.Set<User>()
            .FirstOrDefaultAsync(x =>
                x.Username == username);

        if (user == null ||
            !user.IsActive ||
            !_passwordService.VerifyPassword(
                user,
                request.Password,
                user.PasswordHash))
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        var response =
            await _jwtTokenService.CreateTokenAsync(user);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserResponse>> Me()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var user = await _db.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == userId);

        if (user == null || !user.IsActive)
            return Unauthorized();

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

        return Ok(new CurrentUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            Roles = roles,
            Permissions = permissions
        });
    }
}
