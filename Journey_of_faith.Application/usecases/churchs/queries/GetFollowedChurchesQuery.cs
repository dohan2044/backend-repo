using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.dtos.church;
using Journey_of_faith.Application.exceptions;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries
{
    public class GetFollowedChurchesQuery : IRequest<IEnumerable<ChurchViewDto>>
    {
    }

    public class GetFollowedChurchesHandler : IRequestHandler<GetFollowedChurchesQuery, IEnumerable<ChurchViewDto>>
    {
        private readonly IChurchQueries _churchQueries;
        private readonly ICurrentUserService _currentUserService;

        public GetFollowedChurchesHandler(IChurchQueries churchQueries, ICurrentUserService currentUserService)
        {
            _churchQueries = churchQueries;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ChurchViewDto>> Handle(GetFollowedChurchesQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            return await _churchQueries.GetFollowedChurchesAsync(userId, cancellationToken);
        }
    }
}
