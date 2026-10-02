using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Domain.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.questions.queries;

public class GetCategoriesQuery : IRequest<IEnumerable<QuestionCategoryDto>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<QuestionCategoryDto>>
{
    private readonly IQuestionQueries _queries;
    public GetCategoriesHandler(IQuestionQueries queries) => _queries = queries;
    public async Task<IEnumerable<QuestionCategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        => await _queries.GetQuestionCategoriesAsync(cancellationToken);
}

public class GetCountQuestionQuery : IRequest<int>;

public class GetCountQuestionHandler : IRequestHandler<GetCountQuestionQuery, int>
{
    private readonly IQuestionQueries _queries;
    public GetCountQuestionHandler(IQuestionQueries queries) => _queries = queries;
    public Task<int> Handle(GetCountQuestionQuery request, CancellationToken cancellationToken)
        => _queries.GetQuestionCountAsync(cancellationToken);
}

public class GetDetailsQuestionCategoryQuery : IRequest<QuestionCategoryDto?>
{
    public int Id { get; set; }
}

public class GetDetailsQuestionCategoryHandler : IRequestHandler<GetDetailsQuestionCategoryQuery, QuestionCategoryDto?>
{
    private readonly IQuestionQueries _queries;
    public GetDetailsQuestionCategoryHandler(IQuestionQueries queries) => _queries = queries;
    public Task<QuestionCategoryDto?> Handle(GetDetailsQuestionCategoryQuery request, CancellationToken cancellationToken)
        => _queries.GetQuestionCategoryAsync(request.Id, cancellationToken);
}

public class GetDetailsQuestionLevelCommand : IRequest<QuizLevelDto?>
{
    public int Id { get; set; }
}

public class GetDetailsQuestionLevelHandler : IRequestHandler<GetDetailsQuestionLevelCommand, QuizLevelDto?>
{
    private readonly IQuestionQueries _queries;
    public GetDetailsQuestionLevelHandler(IQuestionQueries queries) => _queries = queries;
    public Task<QuizLevelDto?> Handle(GetDetailsQuestionLevelCommand request, CancellationToken cancellationToken)
        => _queries.GetQuizLevelAsync(request.Id, cancellationToken);
}

public class GetDetailsQuestionTypeQuery : IRequest<QuestionTypeDto?>
{
    public int Id { get; set; }
}

public class GetDetailsQuestionTypeHandler : IRequestHandler<GetDetailsQuestionTypeQuery, QuestionTypeDto?>
{
    private readonly IQuestionQueries _queries;
    public GetDetailsQuestionTypeHandler(IQuestionQueries queries) => _queries = queries;
    public Task<QuestionTypeDto?> Handle(GetDetailsQuestionTypeQuery request, CancellationToken cancellationToken)
        => _queries.GetQuestionTypeAsync(request.Id, cancellationToken);
}

public class GetLevelQuery : IRequest<IEnumerable<QuizLevelDto>>;

public class GetLevelHandler : IRequestHandler<GetLevelQuery, IEnumerable<QuizLevelDto>>
{
    private readonly IQuestionQueries _queries;
    public GetLevelHandler(IQuestionQueries queries) => _queries = queries;
    public async Task<IEnumerable<QuizLevelDto>> Handle(GetLevelQuery request, CancellationToken cancellationToken)
        => await _queries.GetLevelsAsync(cancellationToken);
}

public class GetQuestionTypeQuery : IRequest<IEnumerable<QuestionTypeDto>>;

public class GetQuestionTypeHandler : IRequestHandler<GetQuestionTypeQuery, IEnumerable<QuestionTypeDto>>
{
    private readonly IQuestionQueries _queries;
    public GetQuestionTypeHandler(IQuestionQueries queries) => _queries = queries;
    public async Task<IEnumerable<QuestionTypeDto>> Handle(GetQuestionTypeQuery request, CancellationToken cancellationToken)
        => await _queries.GetQuestionTypesAsync(cancellationToken);
}

public class GetQuestionsQuery : IRequest<PagedResult<QuestionView>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}

public class GetQuestionsHandler : IRequestHandler<GetQuestionsQuery, PagedResult<QuestionView>>
{
    private readonly IQuestionQueries _queries;
    public GetQuestionsHandler(IQuestionQueries queries) => _queries = queries;
    public Task<PagedResult<QuestionView>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
        => _queries.GetQuestionsAsync(request.Page, request.PageSize, request.Search, cancellationToken);
}

public class GetQuestionWithConditionQuery : IRequest<IEnumerable<QuestionView>>
{
    public int CategoryId { get; set; }
    public int LevelId { get; set; }
    public int QuestionCount { get; set; }
}

public class GetQuestionWithConditionHandler : IRequestHandler<GetQuestionWithConditionQuery, IEnumerable<QuestionView>>
{
    private readonly IQuestionQueries _queries;
    public GetQuestionWithConditionHandler(IQuestionQueries queries) => _queries = queries;
    public async Task<IEnumerable<QuestionView>> Handle(GetQuestionWithConditionQuery request, CancellationToken cancellationToken)
        => await _queries.GetQuestionsAsync(request.CategoryId, request.LevelId, request.QuestionCount, cancellationToken);
}
