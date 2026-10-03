
namespace Journey_of_faith.Domain.entities.compete
{
    public class ArenaLevels : AuditableEntity
    {
        private readonly List<ArenaQuestion> _questions = new();
        public string LevelCode { get; set; } = string.Empty;
        public string Title { get;  set; } = string.Empty;
        public string? Subtitle { get;  set; }
        public string? Tag { get;  set; }
        public string? Description { get;  set; }
        public string? FocusTopics { get;  set; }
        public int TotalQuestions { get;  set; } = 10;
        public int TimePerQuestion { get;  set; } = 15;
        public int PassScore { get;  set; } = 8;
        public int ComboBonusPoints { get;  set; } = 50;
        public decimal ComboMultiplier { get;  set; } = 2.0m;
        public int? PrerequisiteLevelId { get;  set; }

        // Navigation
        public virtual ArenaLevels? PrerequisiteLevel { get;  set; }
        public virtual IReadOnlyCollection<ArenaQuestion> Questions => _questions.AsReadOnly();

        public void AddQuestion(ArenaQuestion question) => _questions.Add(question);
    }

    public class ArenaQuestion : AuditableEntity
    {
        private readonly List<ArenaQuestionOption> _options = new();

        public int LevelId { get;  set; }
        public string Content { get;  set; } = string.Empty;
        public string? Explanation { get;  set; }
        public bool IsActive { get;  set; } = true;

        // Navigation
        public virtual ArenaLevels Level { get;  set; } = null!;
        public virtual IReadOnlyCollection<ArenaQuestionOption> Options => _options.AsReadOnly();

        public void AddOption(ArenaQuestionOption option) => _options.Add(option);
    }

    public class ArenaQuestionOption : AuditableEntity
    {
        public int QuestionId { get;  set; }
        public string Content { get;  set; } = string.Empty;
        public bool IsCorrect { get;  set; }
        public short SortOrder { get;  set; }

        public virtual ArenaQuestion Question { get;  set; } = null!;
    }

    public class ArenaUserProgress : AuditableEntity
    {
        public Guid UserId { get;  set; }
        public int LevelId { get;  set; }
        public bool IsPassed { get;  set; }
        public int BestScore { get;  set; }
        public int MaxStreak { get;  set; }
        public DateTime? PassedAt { get;  set; }
        public DateTime UpdatedAt { get;  set; } = DateTime.UtcNow;

        public virtual ArenaLevels Level { get;  set; } = null!;
    }

    public class ArenaAttempt : AuditableEntity
    {
        private readonly List<ArenaAttemptAnswer> _answers = new();

        public Guid UserId { get;  set; }
        public int LevelId { get;  set; }
        public int Score { get;  set; }
        public int TotalCorrect { get;  set; }
        public int MaxStreak { get;  set; }
        public int DurationSeconds { get;  set; }
        public bool IsPassed { get;  set; }
        public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;

        // Navigation
        public virtual ArenaLevels Level { get;  set; } = null!;
        public virtual IReadOnlyCollection<ArenaAttemptAnswer> Answers => _answers.AsReadOnly();

        public void AddAnswer(ArenaAttemptAnswer answer) => _answers.Add(answer);
    }

    public class ArenaAttemptAnswer : AuditableEntity
    {
        public int AttemptId { get;  set; }
        public int QuestionId { get;  set; }
        public int? SelectedOptionId { get;  set; }
        public bool IsCorrect { get;  set; }
        public int TimeSpentSeconds { get;  set; }

        public virtual ArenaAttempt Attempt { get;  set; } = null!;
        public virtual ArenaQuestion Question { get;  set; } = null!;
        public virtual ArenaQuestionOption? SelectedOption { get;  set; }
    }

    public class ArenaAchievement : AuditableEntity
    {
        public string Code { get;  set; } = string.Empty;
        public string Title { get;  set; } = string.Empty;
        public string Description { get;  set; } = string.Empty;
        public string? IconKey { get;  set; }
        public int RequiredValue { get;  set; }
    }

    public class ArenaUserAchievement : AuditableEntity
    {
        public Guid UserId { get;  set; }
        public int AchievementId { get;  set; }
        public DateTime UnlockedAt { get;  set; } = DateTime.UtcNow;

        public virtual ArenaAchievement Achievement { get;  set; } = null!;
    }
}