using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await db.Database.MigrateAsync();

        // ------------------------------------------------------------
        // PERMISSIONS
        // ------------------------------------------------------------
        var permissions = new[]
        {
            ("USERS.VIEW", "View Users", "Users"),
            ("USERS.CREATE", "Create Users", "Users"),
            ("USERS.UPDATE", "Update Users", "Users"),
            ("USERS.DELETE", "Delete Users", "Users"),

            ("ROLES.VIEW", "View Roles", "Roles"),
            ("ROLES.CREATE", "Create Roles", "Roles"),
            ("ROLES.UPDATE", "Update Roles", "Roles"),
            ("ROLES.DELETE", "Delete Roles", "Roles"),

            ("PERMISSIONS.VIEW", "View Permissions", "Permissions"),
            ("PERMISSIONS.CREATE", "Create Permissions", "Permissions"),
            ("PERMISSIONS.UPDATE", "Update Permissions", "Permissions"),
            ("PERMISSIONS.DELETE", "Delete Permissions", "Permissions"),

            ("USER_ROLES.VIEW", "View User Roles", "User Roles"),
            ("USER_ROLES.ASSIGN", "Assign User Roles", "User Roles"),
            ("USER_ROLES.REMOVE", "Remove User Roles", "User Roles"),

            ("ROLE_PERMISSIONS.VIEW", "View Role Permissions", "Role Permissions"),
            ("ROLE_PERMISSIONS.ASSIGN", "Assign Role Permissions", "Role Permissions"),
            ("ROLE_PERMISSIONS.REMOVE", "Remove Role Permissions", "Role Permissions")
        };

        foreach (var item in permissions)
        {
            if (!await db.Permissions.AnyAsync(x => x.Code == item.Item1))
            {
                db.Permissions.Add(new Permission
                {
                    Id = Guid.NewGuid(),
                    Code = item.Item1,
                    Name = item.Item2,
                    Module = item.Item3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await db.SaveChangesAsync();

        // ------------------------------------------------------------
        // SUPER ADMIN ROLE
        // ------------------------------------------------------------
        var superAdminRole = await db.Roles
            .FirstOrDefaultAsync(x => x.Code == "SUPER_ADMIN");

        if (superAdminRole == null)
        {
            superAdminRole = new Role
            {
                Id = Guid.NewGuid(),
                Code = "SUPER_ADMIN",
                Name = "Super Administrator",
                Description = "Full system administration access.",
                IsSystemRole = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            db.Roles.Add(superAdminRole);
            await db.SaveChangesAsync();
        }

        // ------------------------------------------------------------
        // ASSIGN ALL PERMISSIONS TO SUPER ADMIN
        // ------------------------------------------------------------
        var allPermissions = await db.Permissions
            .Where(x => x.IsActive)
            .ToListAsync();

        foreach (var permission in allPermissions)
        {
            var exists = await db.RolePermissions.AnyAsync(x =>
                x.RoleId == superAdminRole.Id &&
                x.PermissionId == permission.Id);

            if (!exists)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id,
                    AssignedAt = DateTime.UtcNow
                });
            }
        }

        await db.SaveChangesAsync();

        // ------------------------------------------------------------
        // ASSIGN SUPER ADMIN ROLE TO ngd.admin
        // ------------------------------------------------------------
        var adminUser = await db.Users
            .FirstOrDefaultAsync(x => x.Username == "ngd.admin");

        if (adminUser != null)
        {
            var userRoleExists = await db.UserRoles.AnyAsync(x =>
                x.UserId == adminUser.Id &&
                x.RoleId == superAdminRole.Id);

            if (!userRoleExists)
            {
                db.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = superAdminRole.Id,
                    AssignedAt = DateTime.UtcNow
                });

                await db.SaveChangesAsync();
            }
        }
    }
}
