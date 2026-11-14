using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public RolesController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // GET ALL ROLES
    // Requires: ROLES.VIEW
    // ============================================================

    [HttpGet]
    [Authorize(Policy = "ROLES.VIEW")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _db.Set<Role>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(roles);
    }

    // ============================================================
    // GET SINGLE ROLE
    // Requires: ROLES.VIEW
    // ============================================================

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ROLES.VIEW")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var role = await _db.Set<Role>()
            .AsNoTracking()
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == id);

        return role == null
            ? NotFound()
            : Ok(role);
    }

    // ============================================================
    // CREATE ROLE
    // Requires: ROLES.CREATE
    // ============================================================

    [HttpPost]
    [Authorize(Policy = "ROLES.CREATE")]
    public async Task<IActionResult> CreateRole(Role role)
    {
        if (string.IsNullOrWhiteSpace(role.Code))
        {
            return BadRequest(new
            {
                message = "Role code is required."
            });
        }

        if (string.IsNullOrWhiteSpace(role.Name))
        {
            return BadRequest(new
            {
                message = "Role name is required."
            });
        }

        var codeExists = await _db.Set<Role>()
            .AnyAsync(x => x.Code == role.Code);

        if (codeExists)
        {
            return Conflict(new
            {
                message = "Role code already exists."
            });
        }

        role.Id = Guid.NewGuid();
        role.CreatedAt = DateTime.UtcNow;

        _db.Set<Role>().Add(role);

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRole),
            new { id = role.Id },
            role);
    }

    // ============================================================
    // UPDATE ROLE
    // Requires: ROLES.UPDATE
    // ============================================================

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ROLES.UPDATE")]
    public async Task<IActionResult> UpdateRole(
        Guid id,
        Role input)
    {
        var role =
            await _db.Set<Role>()
                .FindAsync(id);

        if (role == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(input.Code))
        {
            return BadRequest(new
            {
                message = "Role code is required."
            });
        }

        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return BadRequest(new
            {
                message = "Role name is required."
            });
        }

        var duplicateCode =
            await _db.Set<Role>()
                .AnyAsync(x =>
                    x.Id != id &&
                    x.Code == input.Code);

        if (duplicateCode)
        {
            return Conflict(new
            {
                message = "Role code already exists."
            });
        }

        role.Name = input.Name;
        role.Code = input.Code;
        role.Description = input.Description;
        role.IsSystemRole = input.IsSystemRole;
        role.IsActive = input.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(role);
    }

    // ============================================================
    // DELETE ROLE
    // Requires: ROLES.DELETE
    // ============================================================

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ROLES.DELETE")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role =
            await _db.Set<Role>()
                .FindAsync(id);

        if (role == null)
        {
            return NotFound();
        }

        if (role.IsSystemRole)
        {
            return Conflict(new
            {
                message =
                    "System roles cannot be deleted."
            });
        }

        var hasUsers =
            await _db.Set<UserRole>()
                .AnyAsync(x =>
                    x.RoleId == id);

        if (hasUsers)
        {
            return Conflict(new
            {
                message =
                    "Role cannot be deleted because it is assigned to one or more users."
            });
        }

        var hasPermissions =
            await _db.Set<RolePermission>()
                .AnyAsync(x =>
                    x.RoleId == id);

        if (hasPermissions)
        {
            return Conflict(new
            {
                message =
                    "Role cannot be deleted because permissions are assigned to it."
            });
        }

        _db.Set<Role>().Remove(role);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}