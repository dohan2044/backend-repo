using Microsoft.AspNetCore.Authorization;

namespace Journey_of_faith.Api.authorization;

public class UserPermissionHandler : AuthorizationHandler<UserPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        UserPermissionRequirement requirement
    )
    {
        var permissions = context.User.Claims
            .Any(e => e.Value.Equals(requirement.Permission, 
            StringComparison.OrdinalIgnoreCase));

        if(permissions)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}