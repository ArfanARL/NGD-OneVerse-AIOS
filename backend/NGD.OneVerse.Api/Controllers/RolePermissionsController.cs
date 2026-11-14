using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/role-permissions")]
public class RolePermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public RolePermissionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // GET ROLE-PERMISSION ASSIGNMENTS
    // Requires: ROLE_PERMISSIONS.VIEW
    // ============================================================

    [HttpGet]
    [Authorize(Policy = "ROLE_PERMISSIONS.VIEW")]
    public async Task<IActionResult> GetAssignments()
    {
        var result = await _db.Set<RolePermission>()
            .AsNoTracking()
            .Include(x => x.Role)
            .Include(x => x.Permission)
            .Select(x => new
            {
                x.RoleId,
                Role = x.Role.Name,
                x.PermissionId,
                Permission = x.Permission.Name,
                PermissionCode = x.Permission.Code,
                x.AssignedAt
            })
            .OrderBy(x => x.Role)
            .ThenBy(x => x.Permission)
            .ToListAsync();

        return Ok(result);
    }

    // ============================================================
    // ASSIGN PERMISSION TO ROLE
    // Requires: ROLE_PERMISSIONS.ASSIGN
    // ============================================================

    [HttpPost]
    [Authorize(Policy = "ROLE_PERMISSIONS.ASSIGN")]
    public async Task<IActionResult> Assign(
        RolePermission assignment)
    {
        var roleExists = await _db.Set<Role>()
            .AnyAsync(x => x.Id == assignment.RoleId);

        if (!roleExists)
        {
            return NotFound(new
            {
                message = "Role not found."
            });
        }

        var permissionExists =
            await _db.Set<Permission>()
                .AnyAsync(x =>
                    x.Id == assignment.PermissionId);

        if (!permissionExists)
        {
            return NotFound(new
            {
                message = "Permission not found."
            });
        }

        var alreadyAssigned =
            await _db.Set<RolePermission>()
                .AnyAsync(x =>
                    x.RoleId == assignment.RoleId &&
                    x.PermissionId == assignment.PermissionId);

        if (alreadyAssigned)
        {
            return Conflict(new
            {
                message =
                    "Permission already assigned to this role."
            });
        }

        assignment.AssignedAt = DateTime.UtcNow;

        _db.Set<RolePermission>().Add(assignment);

        await _db.SaveChangesAsync();

        return Created(
            "",
            assignment);
    }

    // ============================================================
    // REMOVE PERMISSION FROM ROLE
    // Requires: ROLE_PERMISSIONS.REMOVE
    // ============================================================

    [HttpDelete("{roleId:guid}/{permissionId:guid}")]
    [Authorize(Policy = "ROLE_PERMISSIONS.REMOVE")]
    public async Task<IActionResult> Remove(
        Guid roleId,
        Guid permissionId)
    {
        var assignment =
            await _db.Set<RolePermission>()
                .FirstOrDefaultAsync(x =>
                    x.RoleId == roleId &&
                    x.PermissionId == permissionId);

        if (assignment == null)
        {
            return NotFound(new
            {
                message =
                    "Role-permission assignment not found."
            });
        }

        // --------------------------------------------------------
        // Protect SUPER_ADMIN
        // --------------------------------------------------------

        var role =
            await _db.Set<Role>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == roleId);

        if (role != null &&
            role.IsSystemRole &&
            role.Code.Equals(
                "SUPER_ADMIN",
                StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new
            {
                message =
                    "Permissions cannot be removed from the SUPER_ADMIN system role."
            });
        }

        _db.Set<RolePermission>().Remove(assignment);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}