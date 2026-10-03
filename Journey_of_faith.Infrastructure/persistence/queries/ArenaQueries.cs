using Dapper;
using Journey_of_faith.Application.common.dtos.arena;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.arena;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.persistence.repositories;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;
using System.Data;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class ArenaQueries : BaseRepository, IArenaQueries
{
    public ArenaQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    // ── Levels + User Progress ────────────────────────────────────────────────
    public Task<IReadOnlyList<ArenaLevelSummaryDto>> GetLevelsAsync(Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<ArenaLevelSummaryDto>>(async connection =>
        {
            var rows = await connection.QueryAsync<ArenaLevelSummaryDto>(new CommandDefinition($@"
                SELECT
                    l.Id, l.LevelCode, l.Title, l.Subtitle, l.Tag,
                    l.TotalQuestions, l.PassScore, l.TimePerQuestion, l.PrerequisiteLevelId,
                    CAST(
                        CASE
                            WHEN l.PrerequisiteLevelId IS NULL THEN 1
                            WHEN EXISTS (
                                SELECT 1 FROM [{_schemaName.Schema}].[{ArenaTable.UserProgress}] p
                                WHERE p.UserId = @UserId AND p.LevelId = l.PrerequisiteLevelId AND p.IsPassed = 1
                            ) THEN 1
                            ELSE 0
                        END AS BIT) AS IsUnlocked,
                    CAST(ISNULL(up.IsPassed, 0) AS BIT) AS IsPassed,
                    ISNULL(up.BestScore, 0) AS BestScore
                FROM [{_schemaName.Schema}].[{ArenaTable.Level}] l
                LEFT JOIN [{_schemaName.Schema}].[{ArenaTable.UserProgress}] up
                    ON up.LevelId = l.Id AND up.UserId = @UserId
                WHERE ISNULL(l.IsDeleted, 0) = 0
                ORDER BY l.Id
            ", new { UserId = userId }, cancellationToken: cancellationToken));

            return rows.ToList();
        });

    // ── Level Detail ──────────────────────────────────────────────────────────
    public Task<ArenaLevelDetailDto?> GetLevelDetailAsync(int levelId, Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync<ArenaLevelDetailDto?>(async connection =>
        {
            return await connection.QuerySingleOrDefaultAsync<ArenaLevelDetailDto>(new CommandDefinition($@"
                SELECT
                    l.Id, l.LevelCode, l.Title, l.Subtitle, l.Description, l.FocusTopics,
                    l.TotalQuestions, l.TimePerQuestion, l.PassScore, l.ComboBonusPoints, l.ComboMultiplier,
                    l.PrerequisiteLevelId,
                    CAST(
                        CASE
                            WHEN l.PrerequisiteLevelId IS NULL THEN 1
                            WHEN EXISTS (
                                SELECT 1 FROM [{_schemaName.Schema}].[{ArenaTable.UserProgress}] p
                                WHERE p.UserId = @UserId AND p.LevelId = l.PrerequisiteLevelId AND p.IsPassed = 1
                            ) THEN 1
                            ELSE 0
                        END AS BIT) AS IsUnlocked,
                    CAST(ISNULL(up.IsPassed, 0) AS BIT) AS IsPassed,
                    ISNULL(up.BestScore, 0) AS BestScore
                FROM [{_schemaName.Schema}].[{ArenaTable.Level}] l
                LEFT JOIN [{_schemaName.Schema}].[{ArenaTable.UserProgress}] up
                    ON up.LevelId = l.Id AND up.UserId = @UserId
                WHERE l.Id = @LevelId AND ISNULL(l.IsDeleted, 0) = 0
            ", new { LevelId = levelId, UserId = userId }, cancellationToken: cancellationToken));
        });

    // ── Attempt Questions (for start & internal scoring) ─────────────────────
    public Task<ArenaStartAttemptDto?> GetAttemptQuestionsAsync(long attemptId, Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync<ArenaStartAttemptDto?>(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(new CommandDefinition($@"
                -- Attempt + Level info
                SELECT
                    a.Id AS AttemptId, a.LevelId,
                    l.Title AS LevelTitle, l.TotalQuestions, l.TimePerQuestion, l.PassScore
                FROM [{_schemaName.Schema}].[{ArenaTable.Attempt}] a
                INNER JOIN [{_schemaName.Schema}].[{ArenaTable.Level}] l ON l.Id = a.LevelId
                WHERE a.Id = @AttemptId AND a.UserId = @UserId;

                -- Questions (random TotalQuestions từ level)
                SELECT TOP (l.TotalQuestions)
                    q.Id, q.Content
                FROM [{_schemaName.Schema}].[{ArenaTable.Question}] q
                INNER JOIN [{_schemaName.Schema}].[{ArenaTable.Attempt}] a ON a.LevelId = q.LevelId
                INNER JOIN [{_schemaName.Schema}].[{ArenaTable.Level}] l ON l.Id = a.LevelId
                WHERE a.Id = @AttemptId AND q.IsActive = 1 AND ISNULL(q.IsDeleted, 0) = 0
                ORDER BY NEWID();

                -- Options (không có IsCorrect)
                SELECT
                    o.Id, o.QuestionId, o.Content, o.SortOrder
                FROM [{_schemaName.Schema}].[{ArenaTable.QuestionOption}] o
                WHERE EXISTS (
                    SELECT 1
                    FROM [{_schemaName.Schema}].[{ArenaTable.Question}] q
                    INNER JOIN [{_schemaName.Schema}].[{ArenaTable.Attempt}] a ON a.LevelId = q.LevelId
                    WHERE a.Id = @AttemptId AND o.QuestionId = q.Id AND q.IsActive = 1
                )
                ORDER BY o.QuestionId, o.SortOrder;
            ", new { AttemptId = attemptId, UserId = userId }, cancellationToken: cancellationToken));

            var header = await multi.ReadSingleOrDefaultAsync<ArenaStartAttemptDto>();
            if (header is null) return null;

            var questions = (await multi.ReadAsync<ArenaQuestionDto>()).ToList();
            var options = (await multi.ReadAsync<ArenaOptionRow>()).ToLookup(o => o.QuestionId);

            foreach (var q in questions)
                q.Options = options[q.Id].Select(o => new ArenaOptionDto
                {
                    Id = o.Id,
                    Content = o.Content,
                    SortOrder = o.SortOrder
                }).ToList();

            header.Questions = questions;
            return header;
        });

    // ── User Progress ─────────────────────────────────────────────────────────
    public Task<IReadOnlyList<ArenaUserProgressDto>> GetUserProgressAsync(Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<ArenaUserProgressDto>>(async connection =>
        {
            var rows = await connection.QueryAsync<ArenaUserProgressDto>(new CommandDefinition($@"
                SELECT
                    p.LevelId, l.Title AS LevelTitle,
                    p.IsPassed, p.BestScore, p.MaxStreak, p.PassedAt
                FROM [{_schemaName.Schema}].[{ArenaTable.UserProgress}] p
                INNER JOIN [{_schemaName.Schema}].[{ArenaTable.Level}] l ON l.Id = p.LevelId
                WHERE p.UserId = @UserId AND ISNULL(l.IsDeleted, 0) = 0
                ORDER BY l.Id
            ", new { UserId = userId }, cancellationToken: cancellationToken));
            return rows.ToList();
        });

    // ── Attempt History ───────────────────────────────────────────────────────
    public Task<IReadOnlyList<ArenaAttemptHistoryDto>> GetAttemptHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<ArenaAttemptHistoryDto>>(async connection =>
        {
            var rows = await connection.QueryAsync<ArenaAttemptHistoryDto>(new CommandDefinition($@"
                SELECT
                    a.Id AS AttemptId, a.LevelId, l.Title AS LevelTitle,
                    a.Score, a.TotalCorrect, a.MaxStreak, a.DurationSeconds, a.IsPassed, a.CreatedAt
                FROM [{_schemaName.Schema}].[{ArenaTable.Attempt}] a
                INNER JOIN [{_schemaName.Schema}].[{ArenaTable.Level}] l ON l.Id = a.LevelId
                WHERE a.UserId = @UserId
                ORDER BY a.CreatedAt DESC
            ", new { UserId = userId }, cancellationToken: cancellationToken));
            return rows.ToList();
        });

    // ── Leaderboard ───────────────────────────────────────────────────────────
    public Task<IReadOnlyList<ArenaLeaderboardEntryDto>> GetLeaderboardAsync(int levelId, int top = 20, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<ArenaLeaderboardEntryDto>>(async connection =>
        {
            var rows = await connection.QueryAsync<ArenaLeaderboardEntryDto>(new CommandDefinition($@"
                SELECT TOP (@Top)
                    ROW_NUMBER() OVER (ORDER BY p.BestScore DESC, p.PassedAt ASC) AS Rank,
                    u.UserName,
                    p.BestScore, p.MaxStreak, p.PassedAt
                FROM [{_schemaName.Schema}].[{ArenaTable.UserProgress}] p
                INNER JOIN AspNetUsers u ON u.Id = CAST(p.UserId AS NVARCHAR(450))
                WHERE p.LevelId = @LevelId
                ORDER BY p.BestScore DESC, p.PassedAt ASC
            ", new { LevelId = levelId, Top = top }, cancellationToken: cancellationToken));
            return rows.ToList();
        });

    // ── Achievements ──────────────────────────────────────────────────────────
    public Task<IReadOnlyList<ArenaAchievementDto>> GetUserAchievementsAsync(Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<ArenaAchievementDto>>(async connection =>
        {
            var rows = await connection.QueryAsync<ArenaAchievementDto>(new CommandDefinition($@"
                SELECT
                    a.Id, a.Code, a.Title, a.Description, a.IconKey,
                    CAST(CASE WHEN ua.UserId IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsUnlocked,
                    ua.UnlockedAt
                FROM [{_schemaName.Schema}].[{ArenaTable.Achievement}] a
                LEFT JOIN [{_schemaName.Schema}].[{ArenaTable.UserAchievement}] ua
                    ON ua.AchievementId = a.Id AND ua.UserId = @UserId
                ORDER BY a.Id
            ", new { UserId = userId }, cancellationToken: cancellationToken));
            return rows.ToList();
        });

    // ── Guard queries ─────────────────────────────────────────────────────────
    public Task<bool> LevelExistsAsync(int levelId, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{ArenaTable.Level}] WHERE Id = @Id AND ISNULL(IsDeleted,0) = 0) THEN 1 ELSE 0 END",
                new { Id = levelId }, cancellationToken: cancellationToken)) > 0);

    public Task<bool> LevelCodeExistsAsync(string levelCode, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{ArenaTable.Level}] WHERE LevelCode = @LevelCode AND ISNULL(IsDeleted,0) = 0) THEN 1 ELSE 0 END",
                new { LevelCode = levelCode }, cancellationToken: cancellationToken)) > 0);

    public Task<bool> AttemptBelongsToUserAsync(long attemptId, Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{ArenaTable.Attempt}] WHERE Id = @AttemptId AND UserId = @UserId) THEN 1 ELSE 0 END",
                new { AttemptId = attemptId, UserId = userId }, cancellationToken: cancellationToken)) > 0);

    public Task<bool> AttemptAlreadySubmittedAsync(long attemptId, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT COUNT(1) FROM [{_schemaName.Schema}].[{ArenaTable.AttemptAnswer}] WHERE AttemptId = @AttemptId",
                new { AttemptId = attemptId }, cancellationToken: cancellationToken)) > 0);

    // ── Private row types ─────────────────────────────────────────────────────
    private sealed class ArenaOptionRow
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public short SortOrder { get; set; }
    }
}
