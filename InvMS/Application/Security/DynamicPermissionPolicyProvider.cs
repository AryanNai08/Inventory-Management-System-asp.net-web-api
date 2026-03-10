using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Application.Security
{
    public class DynamicPermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public DynamicPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // ASP.NET Core provides a default policy provider that we can use as a fallback
            // for standard policies (like [Authorize] without a policy name).
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => 
            FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => 
            FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // First, try to get the policy from the default provider (static policies)
            var policy = FallbackPolicyProvider.GetPolicyAsync(policyName).Result;

            if (policy != null)
            {
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // If the policy is not found, we assume the policyName IS the permission.
            // We create a new policy dynamically that requires this permission.
            var newPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(newPolicy);
        }
    }
}
