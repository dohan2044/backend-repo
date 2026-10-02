namespace Journey_of_faith.Application.common.dtos.arena;

public class ArenaLevelSummaryDto
{
    public int Id { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Tag { get; set; }
    public int TotalQuestions { get; set; }
    public int PassScore { get; set; }
    public int TimePerQuestion { get; set; }
    public int? PrerequisiteLevelId { get; set; }

    // User-specific (null nếu chưa thi)
    public bool IsUnlocked { get; set; }
    public bool IsPassed { get; set; }
    public int BestScore { get; set; }
}

public class ArenaLevelDetailDto
{
    public int Id { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? FocusTopics { get; set; }
    public int TotalQuestions { get; set; }
    public int TimePerQuestion { get; set; }
    public int PassScore { get; set; }
    public int ComboBonusPoints { get; set; }
    public decimal ComboMultiplier { get; set; }
    public int? PrerequisiteLevelId { get; set; }
    public bool IsUnlocked { get; set; }
    public bool IsPassed { get; set; }
    public int BestScore { get; set; }
}

public class ArenaQuestionDto
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<ArenaOptionDto> Options { get; set; } = new();
}

public class ArenaOptionDto
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public short SortOrder { get; set; }
    // IsCorrect KHÔNG expose ra ngoài khi đang thi
}

public class ArenaStartAttemptDto
{
    public long AttemptId { get; set; }
    public int LevelId { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int TimePerQuestion { get; set; }
    public int PassScore { get; set; }
    public List<ArenaQuestionDto> Questions { get; set; } = new();
}

public class ArenaSubmitResultDto
{
    public int Score { get; set; }
    public int TotalCorrect { get; set; }
    public int TotalQuestions { get; set; }
    public int MaxStreak { get; set; }
    public int BonusPoints { get; set; }
    public bool IsPassed { get; set; }
    public bool IsNewBestScore { get; set; }
    public bool IsFirstPass { get; set; }
    public List<string> UnlockedAchievements { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class ArenaUserProgressDto
{
    public int LevelId { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public int BestScore { get; set; }
    public int MaxStreak { get; set; }
    public DateTime? PassedAt { get; set; }
}

public class ArenaAttemptHistoryDto
{
    public long AttemptId { get; set; }
    public int LevelId { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalCorrect { get; set; }
    public int MaxStreak { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ArenaLeaderboardEntryDto
{
    public int Rank { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int BestScore { get; set; }
    public int MaxStreak { get; set; }
    public DateTime? PassedAt { get; set; }
}

public class ArenaAchievementDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconKey { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}
