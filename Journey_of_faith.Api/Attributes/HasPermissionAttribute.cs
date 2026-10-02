
using Microsoft.AspNetCore.Authorization;

namespace Journey_of_faith.Api.Attributes;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission): base(permission) {}
}
