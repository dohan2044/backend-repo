using Journey_of_faith.Application.common.dtos.church;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries;

public class GetLiturgyTodayQuery : IRequest<LiturgyViewDto?>;

public class GetLiturgyTodayHandler : IRequestHandler<GetLiturgyTodayQuery, LiturgyViewDto?>
{
    private readonly IChurchQueries _churchQueries;

    public GetLiturgyTodayHandler(IChurchQueries churchQueries) => _churchQueries = churchQueries;

    public Task<LiturgyViewDto?> Handle(GetLiturgyTodayQuery request, CancellationToken cancellationToken)
        => _churchQueries.GetLiturgyTodayAsync(cancellationToken);
}
