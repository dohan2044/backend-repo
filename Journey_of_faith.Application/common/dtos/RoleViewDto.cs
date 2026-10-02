namespace Journey_of_faith.Application.common.dtos;

public sealed class RoleViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class RolePermissionDto
{
    public Guid RoleId { get; set; } 
    public string RoleName { get; set; } = string.Empty;
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}
