using Journey_of_faith.Application.common.dtos.church;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries;

public class GetDailyWordCommand : IRequest<DailyWordViewDto?>;

public class GetDailyWordHandler : IRequestHandler<GetDailyWordCommand, DailyWordViewDto?>
{
    private readonly IChurchQueries _churchQueries;

    public GetDailyWordHandler(IChurchQueries churchQueries) => _churchQueries = churchQueries;

    public Task<DailyWordViewDto?> Handle(GetDailyWordCommand request, CancellationToken cancellationToken)
        => _churchQueries.GetDailyWordAsync(cancellationToken);
}
