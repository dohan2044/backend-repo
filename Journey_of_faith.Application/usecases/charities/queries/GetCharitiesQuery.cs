using Domain.Entities;
using Journey_of_faith.Application.common.caching.interfaces;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.charities.queries;

public sealed class GetCharitiesQuery : IRequest<IReadOnlyList<CharityActivity>>, ICacheableQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 15;
    public string? TextSearch { get; init; }

    public string CacheGroup => "charities";
    public string CacheKey => $"charities:{TextSearch}:{Page}:{PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}

public sealed class GetCharitiesHandler(ICharityRepository repository)
    : IRequestHandler<GetCharitiesQuery, IReadOnlyList<CharityActivity>>
{
    public Task<IReadOnlyList<CharityActivity>> Handle(
        GetCharitiesQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<CharityActivity>>(
            repository.GetCharities(request.Page, request.PageSize, request.TextSearch).ToList());
}
