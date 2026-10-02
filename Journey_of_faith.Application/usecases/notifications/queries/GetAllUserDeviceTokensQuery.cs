using Journey_of_faith.Application.common.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.notifications.queries;

public sealed record GetAllUserDeviceTokensQuery : IRequest<List<UserDeviceTokenDto>>;

public sealed class GetAllUserDeviceTokensHandler(IUserDeviceQueries userDeviceQueries)
    : IRequestHandler<GetAllUserDeviceTokensQuery, List<UserDeviceTokenDto>>
{
    public Task<List<UserDeviceTokenDto>> Handle(GetAllUserDeviceTokensQuery query, CancellationToken cancellationToken)
        => userDeviceQueries.GetAllUserDeviceTokensAsync(cancellationToken);
}
