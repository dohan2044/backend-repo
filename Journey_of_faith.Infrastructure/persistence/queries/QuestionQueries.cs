using System.Data;
using Dapper;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.questions;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class QuestionQueries : BaseRepository, IQuestionQueries
{
    private static readonly HashSet<string> AllowedTables =
    [
        TableQuestion.QuizLevel,
        TableQuestion.QuestionType,
        TableQuestion.QuestionCategory,
        TableQuestion.Question,
        TableQuestion.Answer
    ];

    public QuestionQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<bool> NameExistsAsync(string name, string table, CancellationToken cancellationToken = default)
    {
        EnsureAllowedTable(table);
        return QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{table}] WHERE Name = @Name) THEN 1 ELSE 0 END",
                new { Name = name }, cancellationToken: cancellationToken)) > 0);
    }

    public Task<QuizLevelDto?> GetQuizLevelAsync(int id, CancellationToken cancellationToken = default)
        => QuerySingleAsync<QuizLevelDto>("GetDetailsQuestionLevel", id, cancellationToken);

    public Task<int> GetQuestionCountByLevelAsync(string name, CancellationToken cancellationToken = default)
        => QueryAsync(async connection => await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "GetCountQuestionLevel", new { Name = name }, commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)));

    public Task<QuestionTypeDto?> GetQuestionTypeAsync(int id, CancellationToken cancellationToken = default)
        => QuerySingleAsync<QuestionTypeDto>("GetDetailsQuestionType", id, cancellationToken);

    public Task<QuestionCategoryDto?> GetQuestionCategoryAsync(int id, CancellationToken cancellationToken = default)
        => QuerySingleAsync<QuestionCategoryDto>("GetDetailsQuestionCategory", id, cancellationToken);

    public Task<IReadOnlyList<QuizLevelDto>> GetLevelsAsync(CancellationToken cancellationToken = default)
        => GetAllAsync<QuizLevelDto>(TableQuestion.QuizLevel, cancellationToken);

    public Task<IReadOnlyList<QuestionTypeDto>> GetQuestionTypesAsync(CancellationToken cancellationToken = default)
        => GetAllAsync<QuestionTypeDto>(TableQuestion.QuestionType, cancellationToken);

    public Task<IReadOnlyList<QuestionCategoryDto>> GetQuestionCategoriesAsync(CancellationToken cancellationToken = default)
        => GetAllAsync<QuestionCategoryDto>(TableQuestion.QuestionCategory, cancellationToken);

    public Task<bool> IdExistsAsync(int id, string table, CancellationToken cancellationToken = default)
    {
        EnsureAllowedTable(table);
        return QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{table}] WHERE Id = @Id) THEN 1 ELSE 0 END",
                new { Id = id }, cancellationToken: cancellationToken)) > 0);
    }

    public Task<PagedResult<QuestionView>> GetQuestionsAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            var questions = (await connection.QueryAsync<QuestionView>(new CommandDefinition(
                "spGetQuestions", new { page, pageSize, search }, commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken))).ToList();
            if (questions.Count > 0)
            {
                var ids = questions.Select(question => question.Id).ToArray();
                var answers = await connection.QueryAsync<AnswerView>(new CommandDefinition(
                    $"SELECT Id, QuestionId, Content, IsCorrect FROM [{_schemaName.Schema}].[{TableQuestion.Answer}] WHERE QuestionId IN @Ids",
                    new { Ids = ids }, cancellationToken: cancellationToken));
                var lookup = answers.ToLookup(answer => answer.QuestionId);
                foreach (var question in questions) question.Answers = lookup[question.Id].ToList();
            }
            var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT COUNT(*) FROM [{_schemaName.Schema}].[{TableQuestion.Question}] WHERE ISNULL(IsDeleted, 0) = 0 AND (@Search IS NULL OR QuestionContent LIKE '%' + @Search + '%')",
                new { Search = string.IsNullOrWhiteSpace(search) ? null : search }, cancellationToken: cancellationToken));
            return new PagedResult<QuestionView>
            {
                Data = questions,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        });

    public Task<bool> QuestionContentExistsAsync(string content, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{TableQuestion.Question}] WHERE QuestionContent = @Content) THEN 1 ELSE 0 END",
                new { Content = content }, cancellationToken: cancellationToken)) > 0);

    public Task<int> GetQuestionCountAsync(CancellationToken cancellationToken = default)
        => QueryAsync(async connection => await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            $"SELECT COUNT(*) FROM [{_schemaName.Schema}].[{TableQuestion.Question}]",
            cancellationToken: cancellationToken)));

    public Task<IReadOnlyList<QuestionView>> GetQuestionsAsync(int categoryId, int levelId, int count, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<QuestionView>>(async connection =>
        {
            var questions = (await connection.QueryAsync<QuestionView>(new CommandDefinition(
                $"SELECT TOP (@Count) Id, LevelId, QuestionContent, TypeId, CategoryId, ImageUrl FROM [{_schemaName.Schema}].[{TableQuestion.Question}] WHERE CategoryId = @CategoryId AND LevelId = @LevelId",
                new { Count = count, CategoryId = categoryId, LevelId = levelId },
                cancellationToken: cancellationToken))).ToList();
            var ids = questions.Select(question => question.Id).ToArray();
            if (ids.Length > 0)
            {
                var answers = await connection.QueryAsync<AnswerView>(new CommandDefinition(
                    $"SELECT Id, QuestionId, Content, IsCorrect FROM [{_schemaName.Schema}].[{TableQuestion.Answer}] WHERE QuestionId IN @Ids",
                    new { Ids = ids }, cancellationToken: cancellationToken));
                var lookup = answers.ToLookup(answer => answer.QuestionId);
                foreach (var question in questions) question.Answers = lookup[question.Id].ToList();
            }
            return questions;
        });

    private Task<T?> QuerySingleAsync<T>(string procedure, int id, CancellationToken cancellationToken)
        where T : class
        => QueryAsync(async connection => await connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            procedure, new { Id = id }, commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)));

    private Task<IReadOnlyList<T>> GetAllAsync<T>(string table, CancellationToken cancellationToken)
    {
        EnsureAllowedTable(table);
        return QueryAsync<IReadOnlyList<T>>(async connection =>
            (await connection.QueryAsync<T>(new CommandDefinition(
                $"SELECT * FROM [{_schemaName.Schema}].[{table}]",
                cancellationToken: cancellationToken))).ToList());
    }

    private static void EnsureAllowedTable(string table)
    {
        if (!AllowedTables.Contains(table)) throw new ArgumentException("Unsupported table.", nameof(table));
    }
}
