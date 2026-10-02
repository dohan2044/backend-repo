using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.dtos.church;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries
{
    public class SearchChurchQuery : IRequest<IEnumerable<ChurchListItemDto>>
    {
        public string? Keyword { get; set; }
        public int? DioceseId { get; set; }
    }

    public class SearchChurchHandler : IRequestHandler<SearchChurchQuery, IEnumerable<ChurchListItemDto>>
    {
        private readonly IChurchQueries _churchQueries;
        private readonly ICurrentUserService _currentUserService;

        public SearchChurchHandler(IChurchQueries churchQueries, ICurrentUserService currentUserService)
        {
            _churchQueries = churchQueries;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ChurchListItemDto>> Handle(SearchChurchQuery request, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsed))
            {
                userId = parsed;
            }

            return await _churchQueries.SearchChurchesAsync(request.Keyword, request.DioceseId, userId, cancellationToken);
        }
    }
}
