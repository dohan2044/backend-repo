using Microsoft.AspNetCore.Authorization;

namespace Journey_of_faith.Api.authorization;

public class UserPermissionRequirement : IAuthorizationRequirement
{
    public string Permission {get;}
    public UserPermissionRequirement(string permission)
    {
        Permission = permission;
    }
}