namespace Journey_of_faith.Application.common.dtos.quiz;

public sealed class QuizViewDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public int QuestionCount { get; set; }
    public bool IsDailyQuiz { get; set; }
    public int TopicId { get; set; }
    public DateTime CreatedTime { get; set; }
    public List<QuizQuestionDto> Questions { get; set; } = [];
}

public sealed class QuizQuestionDto
{
    public int Id { get; set; }
    public string QuestionContent { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<QuizAnswerDto> Answers { get; set; } = [];
}

public sealed class QuizAnswerDto
{
    public int QuestionId { get; set; }
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public sealed class ExamHistoryDto
{
    public string ExamName { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public int CorrectAnswer { get; set; }
    public int Score { get; set; }
}

public sealed class TopicViewDto
{
    public int Id { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int QuizCount { get; set; }
}
