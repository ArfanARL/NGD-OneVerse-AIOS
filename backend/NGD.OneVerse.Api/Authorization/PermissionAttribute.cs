using Microsoft.AspNetCore.Authorization;

namespace NGD.OneVerse.Api.Authorization;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true)]
public sealed class PermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission:";

    public PermissionAttribute(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException(
                "Permission cannot be empty.",
                nameof(permission));

        Policy = $"{PolicyPrefix}{permission}";
    }
}