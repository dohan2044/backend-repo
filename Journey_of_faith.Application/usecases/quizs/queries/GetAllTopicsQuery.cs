using Journey_of_faith.Application.common.dtos.quiz;
using MediatR;

namespace Journey_of_faith.Application.usecases.quizs.queries;

public class GetAllTopicQuery : IRequest<IEnumerable<TopicViewDto>>;

public class GetAllTopicHandler : IRequestHandler<GetAllTopicQuery, IEnumerable<TopicViewDto>>
{
    private readonly IExamQueries _queries;
    public GetAllTopicHandler(IExamQueries queries) => _queries = queries;

    public async Task<IEnumerable<TopicViewDto>> Handle(GetAllTopicQuery request, CancellationToken cancellationToken)
        => await _queries.GetTopicsAsync(cancellationToken);
}
