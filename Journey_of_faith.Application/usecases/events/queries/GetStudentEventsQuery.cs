using Domain.Entities;
using Journey_of_faith.Application.common.caching.interfaces;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.queries;

public sealed class GetStudentEventsQuery : IRequest<IReadOnlyList<StudentEvent>>, ICacheableQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 15;
    public string? TextSearch { get; init; }

    public string CacheGroup => "student-events";
    public string CacheKey => $"student-events:{TextSearch}:{Page}:{PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}

public sealed class GetStudentEventsHandler(IStudentEventRepository repository)
    : IRequestHandler<GetStudentEventsQuery, IReadOnlyList<StudentEvent>>
{
    public Task<IReadOnlyList<StudentEvent>> Handle(
        GetStudentEventsQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<StudentEvent>>(
            repository.GetStudentEvents(request.Page, request.PageSize, request.TextSearch).ToList());
}
