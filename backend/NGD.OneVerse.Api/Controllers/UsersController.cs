using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Api.Services;
using NGD.OneVerse.Domain.Entities;
using PermissionAttribute = NGD.OneVerse.Api.Authorization.PermissionAttribute;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordService _passwordService;

    public UsersController(
        ApplicationDbContext db,
        PasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
    }

    // ============================================================
    // GET ALL USERS
    // Permission: USERS.VIEW
    // ============================================================

    [HttpGet]
    [Permission("USERS.VIEW")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Set<User>()
            .AsNoTracking()
            .OrderBy(x => x.Username)
            .Select(x => new
            {
                x.Id,
                x.Username,
                x.Email,
                x.FirstName,
                x.LastName,
                x.IsActive,
                x.CreatedAt,
                x.UpdatedAt,

                Roles = x.UserRoles
                    .Where(ur => ur.Role.IsActive)
                    .Select(ur => new
                    {
                        ur.RoleId,
                        ur.Role.Code,
                        ur.Role.Name
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(users);
    }

    // ============================================================
    // GET USER
    // Permission: USERS.VIEW
    // ============================================================

    [HttpGet("{id:guid}")]
    [Permission("USERS.VIEW")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _db.Set<User>()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.Username,
                x.Email,
                x.FirstName,
                x.LastName,
                x.IsActive,
                x.CreatedAt,
                x.UpdatedAt,

                Roles = x.UserRoles
                    .Where(ur => ur.Role.IsActive)
                    .Select(ur => new
                    {
                        ur.RoleId,
                        ur.Role.Code,
                        ur.Role.Name
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });

        return Ok(user);
    }

    // ============================================================
    // CREATE USER
    // Permission: USERS.CREATE
    // ============================================================

    [HttpPost]
    [Permission("USERS.CREATE")]
    public async Task<IActionResult> CreateUser(
        CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest(new
            {
                message = "Username is required."
            });

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new
            {
                message = "Email is required."
            });

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new
            {
                message = "Password is required."
            });

        var username = request.Username.Trim();
        var email = request.Email.Trim();

        if (await _db.Set<User>()
            .AnyAsync(x => x.Username == username))
        {
            return Conflict(new
            {
                message = "Username already exists."
            });
        }

        if (await _db.Set<User>()
            .AnyAsync(x => x.Email == email))
        {
            return Conflict(new
            {
                message = "Email already exists."
            });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            FirstName = request.FirstName?.Trim() ?? string.Empty,
            LastName = request.LastName?.Trim() ?? string.Empty,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordService.HashPassword(
                user,
                request.Password);

        _db.Set<User>().Add(user);

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.IsActive,
                user.CreatedAt
            });
    }

    // ============================================================
    // UPDATE USER
    // Permission: USERS.UPDATE
    // ============================================================

    [HttpPut("{id:guid}")]
    [Permission("USERS.UPDATE")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        UpdateUserRequest request)
    {
        var user = await _db.Set<User>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });

        var username = request.Username?.Trim();
        var email = request.Email?.Trim();

        if (!string.IsNullOrWhiteSpace(username) &&
            await _db.Set<User>()
                .AnyAsync(x =>
                    x.Id != id &&
                    x.Username == username))
        {
            return Conflict(new
            {
                message = "Username already exists."
            });
        }

        if (!string.IsNullOrWhiteSpace(email) &&
            await _db.Set<User>()
                .AnyAsync(x =>
                    x.Id != id &&
                    x.Email == email))
        {
            return Conflict(new
            {
                message = "Email already exists."
            });
        }

        if (!string.IsNullOrWhiteSpace(username))
            user.Username = username;

        if (!string.IsNullOrWhiteSpace(email))
            user.Email = email;

        if (request.FirstName != null)
            user.FirstName = request.FirstName.Trim();

        if (request.LastName != null)
            user.LastName = request.LastName.Trim();

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash =
                _passwordService.HashPassword(
                    user,
                    request.Password);
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    // ============================================================
    // ACTIVATE / DEACTIVATE USER
    // Permission: USERS.UPDATE
    // ============================================================

    [HttpPatch("{id:guid}/status")]
    [Permission("USERS.UPDATE")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateUserStatusRequest request)
    {
        var user = await _db.Set<User>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });

        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Username,
            user.IsActive,
            user.UpdatedAt
        });
    }

    // ============================================================
    // DELETE USER
    // Permission: USERS.DELETE
    // ============================================================

    [HttpDelete("{id:guid}")]
    [Permission("USERS.DELETE")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _db.Set<User>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });

        _db.Set<User>().Remove(user);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}


// ================================================================
// REQUEST MODELS
// ================================================================

public sealed class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool IsActive { get; set; } = true;
}


public sealed class UpdateUserRequest
{
    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool? IsActive { get; set; }
}


public sealed class UpdateUserStatusRequest
{
    public bool IsActive { get; set; }
}
