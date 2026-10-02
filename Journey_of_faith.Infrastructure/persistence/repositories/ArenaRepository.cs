using System.Data;
using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.entities.compete;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.repositories;

public class ArenaRepository : BaseRepository, IArenaRepository
{
    private readonly TableSchemaName _name;

    public ArenaRepository(IDbConnectionFactory factory, IOptions<TableSchemaName> options)
        : base(factory, options)
    {
        _name = options.Value;
    }

    // ── Level ─────────────────────────────────────────────────────────────────
    public Task<int> CreateLevelAsync(ArenaLevels level, CancellationToken cancellationToken = default)
        => ExecuteAsync(async connection =>
        {
            return await connection.ExecuteScalarAsync<int>($@"
                INSERT INTO [{_name.Schema}].[{ArenaTable.Level}]
                    (LevelCode, Title, Subtitle, Tag, Description, FocusTopics,
                     TotalQuestions, TimePerQuestion, PassScore, ComboBonusPoints, ComboMultiplier, PrerequisiteLevelId)
                OUTPUT inserted.Id
                VALUES
                    (@LevelCode, @Title, @Subtitle, @Tag, @Description, @FocusTopics,
                     @TotalQuestions, @TimePerQuestion, @PassScore, @ComboBonusPoints, @ComboMultiplier, @PrerequisiteLevelId)
            ", new
            {
                level.LevelCode,
                level.Title,
                level.Subtitle,
                level.Tag,
                level.Description,
                level.FocusTopics,
                level.TotalQuestions,
                level.TimePerQuestion,
                level.PassScore,
                level.ComboBonusPoints,
                level.ComboMultiplier,
                level.PrerequisiteLevelId
            });
        });

    public Task<bool> UpdateLevelAsync(ArenaLevels level, CancellationToken cancellationToken = default)
        => ExecuteAsync(async connection =>
        {
            var rows = await connection.ExecuteAsync($@"
                UPDATE [{_name.Schema}].[{ArenaTable.Level}]
                SET Title = @Title, Subtitle = @Subtitle, Tag = @Tag,
                    Description = @Description, FocusTopics = @FocusTopics,
                    TotalQuestions = @TotalQuestions, TimePerQuestion = @TimePerQuestion,
                    PassScore = @PassScore, ComboBonusPoints = @ComboBonusPoints,
                    ComboMultiplier = @ComboMultiplier, PrerequisiteLevelId = @PrerequisiteLevelId
                WHERE Id = @Id AND ISNULL(IsDeleted, 0) = 0
            ", new
            {
                level.Id,
                level.Title,
                level.Subtitle,
                level.Tag,
                level.Description,
                level.FocusTopics,
                level.TotalQuestions,
                level.TimePerQuestion,
                level.PassScore,
                level.ComboBonusPoints,
                level.ComboMultiplier,
                level.PrerequisiteLevelId
            });
            return rows > 0;
        });

    public Task<bool> DeleteLevelAsync(int levelId, CancellationToken cancellationToken = default)
        => ExecuteAsync(async connection =>
        {
            var rows = await connection.ExecuteAsync($@"
                UPDATE [{_name.Schema}].[{ArenaTable.Level}]
                SET IsDeleted = 1, DeletionTime = GETUTCDATE()
                WHERE Id = @Id AND ISNULL(IsDeleted, 0) = 0
            ", new { Id = levelId });
            return rows > 0;
        });

    // ── Question ──────────────────────────────────────────────────────────────
    public Task<int> AddQuestionAsync(ArenaQuestion question, CancellationToken cancellationToken = default)
        => ExecuteAsync(async connection =>
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var questionId = await connection.ExecuteScalarAsync<int>($@"
                    INSERT INTO [{_name.Schema}].[{ArenaTable.Question}]
                        (LevelId, Content, Explanation, IsActive)
                    OUTPUT inserted.Id
                    VALUES (@LevelId, @Content, @Explanation, @IsActive)
                ", new
                {
                    question.LevelId,
                    question.Content,
                    question.Explanation,
                    question.IsActive
                }, transaction);

                foreach (var option in question.Options)
                {
                    await connection.ExecuteAsync($@"
                        INSERT INTO [{_name.Schema}].[{ArenaTable.QuestionOption}]
                            (QuestionId, Content, IsCorrect, SortOrder)
                        VALUES (@QuestionId, @Content, @IsCorrect, @SortOrder)
                    ", new
                    {
                        QuestionId = questionId,
                        option.Content,
                        option.IsCorrect,
                        option.SortOrder
                    }, transaction);
                }

                transaction.Commit();
                return questionId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });

    // ── Attempt ───────────────────────────────────────────────────────────────
    public Task<long> SaveAttemptAsync(ArenaAttempt attempt, CancellationToken cancellationToken = default)
        => ExecuteAsync(async connection =>
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Upsert attempt (insert hoặc update nếu đã có)
                var attemptId = await connection.ExecuteScalarAsync<long>($@"
                    IF EXISTS (SELECT 1 FROM [{_name.Schema}].[{ArenaTable.Attempt}] WHERE Id = @Id)
                    BEGIN
                        UPDATE [{_name.Schema}].[{ArenaTable.Attempt}]
                        SET Score = @Score, TotalCorrect = @TotalCorrect, MaxStreak = @MaxStreak,
                            DurationSeconds = @DurationSeconds, IsPassed = @IsPassed
                        WHERE Id = @Id;
                        SELECT @Id;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO [{_name.Schema}].[{ArenaTable.Attempt}]
                            (UserId, LevelId, Score, TotalCorrect, MaxStreak, DurationSeconds, IsPassed, CreatedAt)
                        OUTPUT inserted.Id
                        VALUES (@UserId, @LevelId, @Score, @TotalCorrect, @MaxStreak, @DurationSeconds, @IsPassed, GETUTCDATE());
                    END
                ", new
                {
                    attempt.Id,
                    attempt.UserId,
                    attempt.LevelId,
                    attempt.Score,
                    attempt.TotalCorrect,
                    attempt.MaxStreak,
                    attempt.DurationSeconds,
                    attempt.IsPassed
                }, transaction);

                // Insert answers
                foreach (var answer in attempt.Answers)
                {
                    await connection.ExecuteAsync($@"
                        INSERT INTO [{_name.Schema}].[{ArenaTable.AttemptAnswer}]
                            (AttemptId, QuestionId, SelectedOptionId, IsCorrect, TimeSpentSeconds)
                        VALUES (@AttemptId, @QuestionId, @SelectedOptionId, @IsCorrect, @TimeSpentSeconds)
                    ", new
                    {
                        AttemptId = attemptId,
                        answer.QuestionId,
                        answer.SelectedOptionId,
                        answer.IsCorrect,
                        answer.TimeSpentSeconds
                    }, transaction);
                }

                transaction.Commit();
                return attemptId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });

    // ── Progress + Achievement ────────────────────────────────────────────────
    public Task UpsertProgressAndAchievementsAsync(
        ArenaUserProgress progress,
        IReadOnlyList<ArenaUserAchievement> newAchievements,
        CancellationToken cancellationToken = default)
        => ExecuteAsync(async connection =>
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // MERGE progress
                await connection.ExecuteAsync($@"
                    MERGE [{_name.Schema}].[{ArenaTable.UserProgress}] AS target
                    USING (SELECT @UserId AS UserId, @LevelId AS LevelId) AS source
                        ON target.UserId = source.UserId AND target.LevelId = source.LevelId
                    WHEN MATCHED THEN
                        UPDATE SET
                            IsPassed = CASE WHEN @IsPassed = 1 THEN 1 ELSE target.IsPassed END,
                            BestScore = CASE WHEN @BestScore > target.BestScore THEN @BestScore ELSE target.BestScore END,
                            MaxStreak = CASE WHEN @MaxStreak > target.MaxStreak THEN @MaxStreak ELSE target.MaxStreak END,
                            PassedAt = CASE WHEN @IsPassed = 1 AND target.PassedAt IS NULL THEN @PassedAt ELSE target.PassedAt END,
                            UpdatedAt = GETUTCDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (UserId, LevelId, IsPassed, BestScore, MaxStreak, PassedAt, UpdatedAt)
                        VALUES (@UserId, @LevelId, @IsPassed, @BestScore, @MaxStreak, @PassedAt, GETUTCDATE());
                ", new
                {
                    progress.UserId,
                    progress.LevelId,
                    progress.IsPassed,
                    progress.BestScore,
                    progress.MaxStreak,
                    progress.PassedAt
                }, transaction);

                // Insert achievements (chỉ những cái chưa có)
                foreach (var achievement in newAchievements)
                {
                    await connection.ExecuteAsync($@"
                        IF NOT EXISTS (
                            SELECT 1 FROM [{_name.Schema}].[{ArenaTable.UserAchievement}]
                            WHERE UserId = @UserId AND AchievementId = @AchievementId
                        )
                        BEGIN
                            INSERT INTO [{_name.Schema}].[{ArenaTable.UserAchievement}]
                                (UserId, AchievementId, UnlockedAt)
                            VALUES (@UserId, @AchievementId, @UnlockedAt)
                        END
                    ", new
                    {
                        UserId = progress.UserId,
                        achievement.AchievementId,
                        achievement.UnlockedAt
                    }, transaction);
                }

                transaction.Commit();
                return 0; // dummy return để satisfy Func<IDbConnection, Task<T>>
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });
}

public static class ArenaTable
{
    public const string Level = "ArenaLevels";
    public const string Question = "ArenaQuestions";
    public const string QuestionOption = "ArenaQuestionOptions";
    public const string Attempt = "ArenaAttempts";
    public const string AttemptAnswer = "ArenaAttemptAnswers";
    public const string UserProgress = "ArenaUserProgress";
    public const string Achievement = "ArenaAchievements";
    public const string UserAchievement = "ArenaUserAchievements";
}
