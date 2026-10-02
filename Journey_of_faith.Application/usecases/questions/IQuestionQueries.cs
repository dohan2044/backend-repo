using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Domain.dtos;

namespace Journey_of_faith.Application.usecases.questions;

public interface IQuestionQueries
{
    Task<bool> NameExistsAsync(string name, string table, CancellationToken cancellationToken = default);
    Task<QuizLevelDto?> GetQuizLevelAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetQuestionCountByLevelAsync(string name, CancellationToken cancellationToken = default);
    Task<QuestionTypeDto?> GetQuestionTypeAsync(int id, CancellationToken cancellationToken = default);
    Task<QuestionCategoryDto?> GetQuestionCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuizLevelDto>> GetLevelsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuestionTypeDto>> GetQuestionTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuestionCategoryDto>> GetQuestionCategoriesAsync(CancellationToken cancellationToken = default);
    Task<bool> IdExistsAsync(int id, string table, CancellationToken cancellationToken = default);
    Task<PagedResult<QuestionView>> GetQuestionsAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<bool> QuestionContentExistsAsync(string content, CancellationToken cancellationToken = default);
    Task<int> GetQuestionCountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QuestionView>> GetQuestionsAsync(int categoryId, int levelId, int count, CancellationToken cancellationToken = default);
}
