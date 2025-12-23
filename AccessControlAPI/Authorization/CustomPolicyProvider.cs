using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AccessControlAPI.Authorization
{
    public class CustomPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Tự động tạo Policy cho mọi tên
            return Task.FromResult<AuthorizationPolicy?>(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build()
            );
        }
    }
}