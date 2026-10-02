using Journey_of_faith.Application.common.dtos.quiz;

namespace Journey_of_faith.Application.usecases.quizs;

public interface IExamQueries
{
    Task<IReadOnlyList<QuizViewDto>> GetAllQuizzesAsync(CancellationToken cancellationToken = default);
    Task<QuizViewDto?> GetQuizDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExamHistoryDto>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TopicViewDto>> GetTopicsAsync(CancellationToken cancellationToken = default);
    Task<bool> TopicNameExistsAsync(string name, CancellationToken cancellationToken = default);
}
