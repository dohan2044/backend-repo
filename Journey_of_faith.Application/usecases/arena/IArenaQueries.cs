using Journey_of_faith.Application.common.dtos.arena;

namespace Journey_of_faith.Application.usecases.arena;

public interface IArenaQueries
{
    Task<IReadOnlyList<ArenaLevelSummaryDto>> GetLevelsAsync(int userId, CancellationToken cancellationToken = default);
    Task<ArenaLevelDetailDto?> GetLevelDetailAsync(int levelId, int userId, CancellationToken cancellationToken = default);
    Task<ArenaStartAttemptDto?> GetAttemptQuestionsAsync(long attemptId, int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArenaUserProgressDto>> GetUserProgressAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArenaAttemptHistoryDto>> GetAttemptHistoryAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArenaLeaderboardEntryDto>> GetLeaderboardAsync(int levelId, int top = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArenaAchievementDto>> GetUserAchievementsAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> LevelExistsAsync(int levelId, CancellationToken cancellationToken = default);
    Task<bool> LevelCodeExistsAsync(string levelCode, CancellationToken cancellationToken = default);
    Task<bool> AttemptBelongsToUserAsync(long attemptId, int userId, CancellationToken cancellationToken = default);
    Task<bool> AttemptAlreadySubmittedAsync(long attemptId, CancellationToken cancellationToken = default);
}
