using Journey_of_faith.Application.common.dtos.church;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries;


public class GetMassScheduleTodayQuery : IRequest<IReadOnlyList<MassScheduleTodayDto>>
{
    public bool NextDay { get; init; }
    public string? Province {get; set;}
}


public class GetMassScheduleTodayHandler : IRequestHandler<GetMassScheduleTodayQuery, IReadOnlyList<MassScheduleTodayDto>>
{
    private readonly IChurchQueries churchQueries;
    public GetMassScheduleTodayHandler(IChurchQueries churchQueries)
    {
        this.churchQueries = churchQueries;
    }

    public async Task<IReadOnlyList<MassScheduleTodayDto>> Handle(GetMassScheduleTodayQuery request, CancellationToken cancellationToken)
    {
        return await churchQueries.GetMassScheduleTodayViewsAsync(request.NextDay,request.Province ?? null, cancellationToken);
    }
}
