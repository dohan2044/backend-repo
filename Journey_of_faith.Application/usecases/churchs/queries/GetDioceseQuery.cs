using Journey_of_faith.Application.common.dtos.church;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries;

public class GetDioceseQuery : IRequest<IEnumerable<DioceseViewDto>>;

public class GetDioceseHandler : IRequestHandler<GetDioceseQuery, IEnumerable<DioceseViewDto>>
{
    private readonly IChurchQueries _churchQueries;

    public GetDioceseHandler(IChurchQueries churchQueries) => _churchQueries = churchQueries;

    public async Task<IEnumerable<DioceseViewDto>> Handle(
        GetDioceseQuery request,
        CancellationToken cancellationToken)
        => await _churchQueries.GetAllDiocesesAsync(cancellationToken);
}
