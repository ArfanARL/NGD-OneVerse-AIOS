using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/user-roles")]
public class UserRolesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public UserRolesController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // GET USER-ROLE ASSIGNMENTS
    // Requires: USER_ROLES.VIEW
    // ============================================================

    [HttpGet]
    [Authorize(Policy = "USER_ROLES.VIEW")]
    public async Task<IActionResult> GetAssignments()
    {
        var result = await _db.Set<UserRole>()
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Role)
            .Select(x => new
            {
                x.UserId,
                Username = x.User.Username,
                x.RoleId,
                Role = x.Role.Name,
                x.AssignedAt
            })
            .OrderBy(x => x.Username)
            .ThenBy(x => x.Role)
            .ToListAsync();

        return Ok(result);
    }

    // ============================================================
    // ASSIGN ROLE TO USER
    // Requires: USER_ROLES.ASSIGN
    // ============================================================

    [HttpPost]
    [Authorize(Policy = "USER_ROLES.ASSIGN")]
    public async Task<IActionResult> Assign(
        UserRole assignment)
    {
        var userExists = await _db.Set<User>()
            .AnyAsync(x => x.Id == assignment.UserId);

        if (!userExists)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        var roleExists = await _db.Set<Role>()
            .AnyAsync(x => x.Id == assignment.RoleId);

        if (!roleExists)
        {
            return NotFound(new
            {
                message = "Role not found."
            });
        }

        var alreadyAssigned =
            await _db.Set<UserRole>()
                .AnyAsync(x =>
                    x.UserId == assignment.UserId &&
                    x.RoleId == assignment.RoleId);

        if (alreadyAssigned)
        {
            return Conflict(new
            {
                message = "Role already assigned to this user."
            });
        }

        assignment.AssignedAt = DateTime.UtcNow;

        _db.Set<UserRole>().Add(assignment);

        await _db.SaveChangesAsync();

        return Created(
            "",
            assignment);
    }

    // ============================================================
    // REMOVE ROLE FROM USER
    // Requires: USER_ROLES.REMOVE
    // ============================================================

    [HttpDelete("{userId:guid}/{roleId:guid}")]
    [Authorize(Policy = "USER_ROLES.REMOVE")]
    public async Task<IActionResult> Remove(
        Guid userId,
        Guid roleId)
    {
        var assignment =
            await _db.Set<UserRole>()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId);

        if (assignment == null)
        {
            return NotFound(new
            {
                message =
                    "User-role assignment not found."
            });
        }

        // Prevent removing the last role from the
        // main administrator account.

        var user =
            await _db.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

        if (user != null &&
            user.Username.Equals(
                "ngd.admin",
                StringComparison.OrdinalIgnoreCase))
        {
            var roleCount =
                await _db.Set<UserRole>()
                    .CountAsync(x =>
                        x.UserId == userId);

            if (roleCount <= 1)
            {
                return Conflict(new
                {
                    message =
                        "The primary administrator must retain at least one role."
                });
            }
        }

        _db.Set<UserRole>().Remove(assignment);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}