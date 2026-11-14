using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NGD.OneVerse.Api.Authorization;

public sealed class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "Permission:";

    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (policyName.StartsWith(
            PolicyPrefix,
            StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName[
                PolicyPrefix.Length..];

            if (string.IsNullOrWhiteSpace(permission))
                return null;

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(
                    new PermissionRequirement(permission))
                .Build();

            return await Task.FromResult(policy);
        }

        return await base.GetPolicyAsync(policyName);
    }
}
