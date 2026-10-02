namespace Journey_of_faith.Application.common.dtos;

public sealed class UserDeviceInfoDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public List<UserDeviceDto> Devices { get; set; } = [];
}

public sealed class UserDeviceDto
{
    public int Id { get; set; }
    public string? DeviceName { get; set; }
    public DateTime CreatedAt { get; set; }
}
