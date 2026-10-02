using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Api.authorization;

public class UserPermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public UserPermissionPolicyProvider(IOptions<AuthorizationOptions> options): base(options) {}

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);
        if(policy != null) return policy;

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new UserPermissionRequirement(policyName))
            .Build();
    } 
}