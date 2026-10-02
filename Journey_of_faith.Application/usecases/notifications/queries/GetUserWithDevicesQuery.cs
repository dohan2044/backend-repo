using Journey_of_faith.Application.common.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.notifications.queries;

public sealed record GetUserWithDevicesQuery(Guid UserId) : IRequest<UserDeviceInfoDto?>;

public sealed class GetUserWithDevicesHandler(IUserDeviceQueries userDeviceQueries)
    : IRequestHandler<GetUserWithDevicesQuery, UserDeviceInfoDto?>
{
    public Task<UserDeviceInfoDto?> Handle(GetUserWithDevicesQuery query, CancellationToken cancellationToken)
        => userDeviceQueries.GetUserWithDevicesAsync(query.UserId, cancellationToken);
}
