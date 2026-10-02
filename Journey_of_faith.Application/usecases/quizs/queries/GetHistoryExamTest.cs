using Journey_of_faith.Application.common.dtos.quiz;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using MediatR;

namespace Journey_of_faith.Application.usecases.quizs.queries;

public class GetHistoryExamTestQuery : IRequest<IReadOnlyList<ExamHistoryDto>>;

public class GetHistoryExamTestHandler : IRequestHandler<GetHistoryExamTestQuery, IReadOnlyList<ExamHistoryDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IExamQueries _queries;

    public GetHistoryExamTestHandler(IExamQueries queries, ICurrentUserService currentUserService)
    {
        _queries = queries;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<ExamHistoryDto>> Handle(GetHistoryExamTestQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            throw new UnauthorizationException("Thong tin tai khoan khong hop le");
        return _queries.GetHistoryAsync(userId, cancellationToken);
    }
}
