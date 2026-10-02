using Journey_of_faith.Domain.entities.compete;

namespace Journey_of_faith.Domain.interfaces;

public interface IArenaRepository
{
    // Level management
    Task<int> CreateLevelAsync(ArenaLevels level, CancellationToken cancellationToken = default);
    Task<bool> UpdateLevelAsync(ArenaLevels level, CancellationToken cancellationToken = default);
    Task<bool> DeleteLevelAsync(int levelId, CancellationToken cancellationToken = default);

    // Question management
    Task<int> AddQuestionAsync(ArenaQuestion question, CancellationToken cancellationToken = default);

    // Attempt
    Task<long> SaveAttemptAsync(ArenaAttempt attempt, CancellationToken cancellationToken = default);

    // Progress + Achievement (upsert trong 1 transaction)
    Task UpsertProgressAndAchievementsAsync(
        ArenaUserProgress progress,
        IReadOnlyList<ArenaUserAchievement> newAchievements,
        CancellationToken cancellationToken = default);
}
