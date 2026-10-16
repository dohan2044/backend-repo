using Journey_of_faith.Application.common.dtos.church;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries;


public class GetMassScheduleQuery : IRequest<IEnumerable<MassScheduleDto>>
{
    public string? Time {get; set;} = string.Empty;
    public string? Name {get; set;} = string.Empty;
    public string? Province {get; set;} = string.Empty;
}

public class GetMassScheduleHandler : IRequestHandler<GetMassScheduleQuery, IEnumerable<MassScheduleDto>>
{
    private readonly IChurchQueries queries;
    public GetMassScheduleHandler(IChurchQueries churchQueries)
    {
        this.queries = churchQueries;
    }

    public async Task<IEnumerable<MassScheduleDto>> Handle(GetMassScheduleQuery query, CancellationToken cancellationToken = default)
    {
        return await queries.GetMassSchedulesAsync(query.Time, query.Name, query.Province, cancellationToken);
    }
}