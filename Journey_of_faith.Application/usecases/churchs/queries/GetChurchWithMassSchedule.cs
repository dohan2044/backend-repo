using Journey_of_faith.Application.common.dtos.church;
using Journey_of_faith.Domain.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries;

public class GetChurchWithMassScheduleQueries : IRequest<PagedResult<ChurchViewDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}

public class GetChurchWithMassScheduleHandler
    : IRequestHandler<GetChurchWithMassScheduleQueries, PagedResult<ChurchViewDto>>
{
    private readonly IChurchQueries _churchQueries;

    public GetChurchWithMassScheduleHandler(IChurchQueries churchQueries) => _churchQueries = churchQueries;

    public Task<PagedResult<ChurchViewDto>> Handle(
        GetChurchWithMassScheduleQueries request,
        CancellationToken cancellationToken)
        => _churchQueries.GetChurchesAsync(request.Page, request.PageSize, request.Search, cancellationToken);
}
