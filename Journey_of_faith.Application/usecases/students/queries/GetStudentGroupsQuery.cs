using Domain.Entities;
using Journey_of_faith.Application.common.caching.interfaces;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.students.queries;

public sealed class GetStudentGroupsQuery : IRequest<IReadOnlyList<StudentGroup>>, ICacheableQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 15;
    public string? TextSearch { get; init; }

    public string CacheGroup => "students-group";
    public string CacheKey => $"students-group:{TextSearch}:{Page}:{PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}

public sealed class GetStudentGroupsHandler(IStudentGroupRepository repository)
    : IRequestHandler<GetStudentGroupsQuery, IReadOnlyList<StudentGroup>>
{
    public Task<IReadOnlyList<StudentGroup>> Handle(
        GetStudentGroupsQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<StudentGroup>>(
            repository.Groups(request.Page, request.PageSize, request.TextSearch).ToList());
}
