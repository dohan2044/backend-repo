using System.Data;
using Dapper;
using Journey_of_faith.Application.common.dtos.quiz;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.quizs;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.dtos.quiz;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class ExamQueries : BaseRepository, IExamQueries
{
    public ExamQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<IReadOnlyList<QuizViewDto>> GetAllQuizzesAsync(CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<QuizViewDto>>(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
                $"""
                SELECT Id, Title, Description, TimeLimit, QuestionCount, IsDailyQuiz, TopicId, CreatedTime
                FROM [{_schemaName.Schema}].[{QuizTalbe.Quiz}]
                WHERE ISNULL(IsDeleted, 0) = 0
                ORDER BY CreatedTime DESC, Id DESC;

                SELECT qq.QuizId, q.Id, q.QuestionContent, q.ImageUrl
                FROM [{_schemaName.Schema}].[{QuizTalbe.QuizQuestion}] qq
                INNER JOIN [{_schemaName.Schema}].[{TableQuestion.Question}] q ON q.Id = qq.QuestionId
                INNER JOIN [{_schemaName.Schema}].[{QuizTalbe.Quiz}] quiz ON quiz.Id = qq.QuizId
                WHERE ISNULL(quiz.IsDeleted, 0) = 0 AND ISNULL(q.IsDeleted, 0) = 0
                ORDER BY qq.QuizId, qq.OrderIndex, qq.Id;

                SELECT answer.QuestionId, answer.Id, answer.Content, answer.IsCorrect
                FROM [{_schemaName.Schema}].[{TableQuestion.Answer}] answer
                WHERE EXISTS (
                    SELECT 1 FROM [{_schemaName.Schema}].[{QuizTalbe.QuizQuestion}] qq
                    INNER JOIN [{_schemaName.Schema}].[{QuizTalbe.Quiz}] quiz ON quiz.Id = qq.QuizId
                    WHERE qq.QuestionId = answer.QuestionId AND ISNULL(quiz.IsDeleted, 0) = 0)
                ORDER BY answer.QuestionId, answer.Id;
                """,
                cancellationToken: cancellationToken));

            var quizzes = (await multi.ReadAsync<QuizViewDto>()).ToList();
            var questionRows = (await multi.ReadAsync<QuizQuestionRow>()).ToList();
            var answers = (await multi.ReadAsync<QuizAnswerDto>()).ToLookup(answer => answer.QuestionId);
            var questions = questionRows.Select(row => new
            {
                row.QuizId,
                Question = new QuizQuestionDto
                {
                    Id = row.Id,
                    QuestionContent = row.QuestionContent,
                    ImageUrl = row.ImageUrl,
                    Answers = answers[row.Id].ToList()
                }
            }).ToLookup(item => item.QuizId, item => item.Question);
            foreach (var quiz in quizzes) quiz.Questions = questions[quiz.Id].ToList();
            return quizzes;
        });

    public Task<QuizViewDto?> GetQuizDetailsAsync(int id, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
                "GetDetails",
                new { Id = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            var quiz = await multi.ReadSingleOrDefaultAsync<QuizViewDto>();
            if (quiz is null) return null;
            var questions = (await multi.ReadAsync<QuizQuestionDto>()).ToList();
            var answers = (await multi.ReadAsync<QuizAnswerDto>()).ToLookup(answer => answer.QuestionId);
            foreach (var question in questions) question.Answers = answers[question.Id].ToList();
            quiz.Questions = questions;
            return quiz;
        });

    public Task<IReadOnlyList<ExamHistoryDto>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<ExamHistoryDto>>(async connection =>
        {
            var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(
                "spGetHistoryExam",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            return rows.Select(row => new ExamHistoryDto
            {
                ExamName = row.NameExam,
                QuestionCount = row.QuestionCount,
                StartDate = row.StartTime.ToString("dd-MM-yyyy HH:mm"),
                CorrectAnswer = row.CorrectAnswer,
                Score = row.Score
            }).ToList();
        });

    public Task<IReadOnlyList<TopicViewDto>> GetTopicsAsync(CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<TopicViewDto>>(async connection =>
            (await connection.QueryAsync<TopicViewDto>(new CommandDefinition(
                $"SELECT Id, TopicName, QuizCount FROM [{_schemaName.Schema}].[{QuizTalbe.Topic}] WHERE ISNULL(IsDeleted, 0) = 0",
                cancellationToken: cancellationToken))).ToList());

    public Task<bool> TopicNameExistsAsync(string name, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{QuizTalbe.Topic}] WHERE TopicName = @Name AND ISNULL(IsDeleted, 0) = 0) THEN 1 ELSE 0 END",
                new { Name = name },
                cancellationToken: cancellationToken)) > 0);

    private sealed class QuizQuestionRow
    {
        public int QuizId { get; set; }
        public int Id { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
