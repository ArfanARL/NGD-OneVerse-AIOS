using Microsoft.AspNetCore.Authorization;

namespace NGD.OneVerse.Api.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy =
            $"{PermissionPolicyProvider.PolicyPrefix}{permission}";
    }
}