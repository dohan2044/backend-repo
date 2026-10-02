using Journey_of_faith.Application.common.dtos.quiz;
using MediatR;

namespace Journey_of_faith.Application.usecases.quizs.queries;

public class GetDetailsQuizQuery : IRequest<QuizViewDto?>
{
    public int Id { get; set; }
}

public class GetDetailsQuizHandler : IRequestHandler<GetDetailsQuizQuery, QuizViewDto?>
{
    private readonly IExamQueries _queries;
    public GetDetailsQuizHandler(IExamQueries queries) => _queries = queries;

    public Task<QuizViewDto?> Handle(GetDetailsQuizQuery request, CancellationToken cancellationToken)
        => _queries.GetQuizDetailsAsync(request.Id, cancellationToken);
}
