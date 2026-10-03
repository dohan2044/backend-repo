using Journey_of_faith.Application.common.caching.interfaces;
using Journey_of_faith.Application.common.dtos.church;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries;

public class GetChurchDetailsQuery : IRequest<ChurchViewDto?>, ICacheableQuery
{
    public int Id { get; set; }
    public string CacheGroup => "churches";
    public string CacheKey => $"church:{Id}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}

public class GetChurchDetailsHandler : IRequestHandler<GetChurchDetailsQuery, ChurchViewDto?>
{
    private readonly IChurchQueries _churchQueries;

    public GetChurchDetailsHandler(IChurchQueries churchQueries) => _churchQueries = churchQueries;

    public Task<ChurchViewDto?> Handle(GetChurchDetailsQuery request, CancellationToken cancellationToken)
        => _churchQueries.GetChurchByIdAsync(request.Id, cancellationToken);
}
