using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PermissionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // GET ALL PERMISSIONS
    // Requires: PERMISSIONS.VIEW
    // ============================================================

    [HttpGet]
    [Authorize(Policy = "PERMISSIONS.VIEW")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _db.Set<Permission>()
            .AsNoTracking()
            .OrderBy(x => x.Module)
            .ThenBy(x => x.Name)
            .ToListAsync();

        return Ok(permissions);
    }

    // ============================================================
    // GET SINGLE PERMISSION
    // Requires: PERMISSIONS.VIEW
    // ============================================================

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PERMISSIONS.VIEW")]
    public async Task<IActionResult> GetPermission(Guid id)
    {
        var permission = await _db.Set<Permission>()
            .AsNoTracking()
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        return permission == null
            ? NotFound()
            : Ok(permission);
    }

    // ============================================================
    // CREATE PERMISSION
    // Requires: PERMISSIONS.CREATE
    // ============================================================

    [HttpPost]
    [Authorize(Policy = "PERMISSIONS.CREATE")]
    public async Task<IActionResult> CreatePermission(
        Permission permission)
    {
        if (string.IsNullOrWhiteSpace(permission.Code))
        {
            return BadRequest(new
            {
                message = "Permission code is required."
            });
        }

        if (string.IsNullOrWhiteSpace(permission.Name))
        {
            return BadRequest(new
            {
                message = "Permission name is required."
            });
        }

        var codeExists = await _db.Set<Permission>()
            .AnyAsync(x => x.Code == permission.Code);

        if (codeExists)
        {
            return Conflict(new
            {
                message = "Permission code already exists."
            });
        }

        permission.Id = Guid.NewGuid();
        permission.CreatedAt = DateTime.UtcNow;

        _db.Set<Permission>().Add(permission);

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPermission),
            new { id = permission.Id },
            permission);
    }

    // ============================================================
    // UPDATE PERMISSION
    // Requires: PERMISSIONS.UPDATE
    // ============================================================

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "PERMISSIONS.UPDATE")]
    public async Task<IActionResult> UpdatePermission(
        Guid id,
        Permission input)
    {
        var permission =
            await _db.Set<Permission>()
                .FindAsync(id);

        if (permission == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(input.Code))
        {
            return BadRequest(new
            {
                message = "Permission code is required."
            });
        }

        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return BadRequest(new
            {
                message = "Permission name is required."
            });
        }

        var duplicateCode =
            await _db.Set<Permission>()
                .AnyAsync(x =>
                    x.Id != id &&
                    x.Code == input.Code);

        if (duplicateCode)
        {
            return Conflict(new
            {
                message = "Permission code already exists."
            });
        }

        permission.Name = input.Name;
        permission.Code = input.Code;
        permission.Description = input.Description;
        permission.Module = input.Module;
        permission.IsActive = input.IsActive;
        permission.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(permission);
    }

    // ============================================================
    // DELETE PERMISSION
    // Requires: PERMISSIONS.DELETE
    // ============================================================

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "PERMISSIONS.DELETE")]
    public async Task<IActionResult> DeletePermission(Guid id)
    {
        var permission =
            await _db.Set<Permission>()
                .FindAsync(id);

        if (permission == null)
        {
            return NotFound();
        }

        var isAssigned =
            await _db.Set<RolePermission>()
                .AnyAsync(x =>
                    x.PermissionId == id);

        if (isAssigned)
        {
            return Conflict(new
            {
                message =
                    "Permission cannot be deleted because it is assigned to one or more roles."
            });
        }

        _db.Set<Permission>().Remove(permission);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}