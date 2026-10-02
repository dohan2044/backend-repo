namespace Journey_of_faith.Application.common.dtos;

public sealed class UserDeviceTokenDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string TokenDevice { get; set; } = string.Empty;
}
