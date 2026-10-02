using Journey_of_faith.Application.common.dtos.quiz;
using MediatR;

namespace Journey_of_faith.Application.usecases.quizs.queries;

public class GetAllQuizzesQuery : IRequest<IEnumerable<QuizViewDto>>;

public class GetAllQuizzesHandler : IRequestHandler<GetAllQuizzesQuery, IEnumerable<QuizViewDto>>
{
    private readonly IExamQueries _queries;
    public GetAllQuizzesHandler(IExamQueries queries) => _queries = queries;

    public async Task<IEnumerable<QuizViewDto>> Handle(GetAllQuizzesQuery request, CancellationToken cancellationToken)
        => await _queries.GetAllQuizzesAsync(cancellationToken);
}
