using Journey_of_faith.Application.common.dtos;

namespace Journey_of_faith.Application.usecases.notifications;

public interface IUserDeviceQueries
{
    Task<UserDeviceInfoDto?> GetUserWithDevicesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<UserDeviceTokenDto>> GetAllUserDeviceTokensAsync(CancellationToken cancellationToken = default);
}
