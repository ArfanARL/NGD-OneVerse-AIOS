using Microsoft.AspNetCore.Authorization;

namespace NGD.OneVerse.Api.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return Task.CompletedTask;

        // Super Administrator bypasses individual permission checks.
        if (context.User.IsInRole("SUPER_ADMIN"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var hasPermission = context.User.Claims.Any(c =>
            c.Type == "permission" &&
            string.Equals(
                c.Value,
                requirement.Permission,
                StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
