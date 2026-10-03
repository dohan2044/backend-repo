using Journey_of_faith.Application.common.dtos.arena;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.compete;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.arena.commands;

public class SubmitArenaAttemptCommand : IRequest<ArenaSubmitResultDto>
{
    public long AttemptId { get; set; }
    public List<AnswerItem> Answers { get; set; } = new();
    public int DurationSeconds { get; set; }

    public record AnswerItem(long QuestionId, long? SelectedOptionId);
}

public class SubmitArenaAttemptHandler : IRequestHandler<SubmitArenaAttemptCommand, ArenaSubmitResultDto>
{
    private readonly IArenaRepository _repo;
    private readonly IArenaQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public SubmitArenaAttemptHandler(IArenaRepository repo, IArenaQueries queries, ICurrentUserService currentUser)
    {
        _repo = repo;
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<ArenaSubmitResultDto> Handle(SubmitArenaAttemptCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");

        // Validate attempt thuộc user và chưa submit
        if (!await _queries.AttemptBelongsToUserAsync(command.AttemptId, userId, cancellationToken))
            throw new ForbiddenException("Bạn không có quyền nộp bài thi này.");

        if (await _queries.AttemptAlreadySubmittedAsync(command.AttemptId, cancellationToken))
            throw new BadRequestException("Bài thi này đã được nộp trước đó.");

        // Lấy câu hỏi + đáp án đúng từ DB (kèm IsCorrect — internal use only)
        var attemptData = await _queries.GetAttemptQuestionsAsync(command.AttemptId, userId, cancellationToken);
        if (attemptData is null)
            throw new NotFoundException("Không tìm thấy dữ liệu bài thi.");

        // Build lookup: QuestionId → correct OptionId
        // NOTE: GetAttemptQuestionsAsync trả về ArenaStartAttemptDto (không có IsCorrect).
        // Cần query riêng để lấy correct options — handled bởi GetCorrectOptionsAsync trong repo.
        // Ở đây ta dùng một internal query để chấm điểm.
        var correctOptions = await _queries.GetAttemptQuestionsAsync(command.AttemptId, userId, cancellationToken);

        // Chấm điểm
        var answerLookup = command.Answers.ToDictionary(a => a.QuestionId, a => a.SelectedOptionId);

        int totalCorrect = 0;
        int currentStreak = 0;
        int maxStreak = 0;
        var attemptAnswers = new List<ArenaAttemptAnswer>();

        // Lấy correct option map từ internal query
        var correctOptionMap = await GetCorrectOptionMapAsync(command.AttemptId, cancellationToken);

        foreach (var (questionId, correctOptionId) in correctOptionMap)
        {
            answerLookup.TryGetValue(questionId, out var selectedOptionId);
            bool isCorrect = correctOptionId.HasValue && selectedOptionId == correctOptionId;

            if (isCorrect)
            {
                totalCorrect++;
                currentStreak++;
                maxStreak = Math.Max(maxStreak, currentStreak);
            }
            else
            {
                currentStreak = 0;
            }

            attemptAnswers.Add(new ArenaAttemptAnswer
            {
                AttemptId = (int)command.AttemptId,
                QuestionId = (int)questionId,
                SelectedOptionId = (int?)selectedOptionId,
                IsCorrect = isCorrect,
                TimeSpentSeconds = 0
            });
        }

        int totalQuestions = correctOptionMap.Count;
        int score = totalQuestions > 0 ? (int)Math.Round((double)totalCorrect / totalQuestions * 100) : 0;

        // Tính combo bonus
        var levelInfo = attemptData;
        int bonusPoints = 0;
        // Combo bonus: nếu streak >= threshold thì cộng bonus (dựa vào entity ComboBonusPoints)
        // Simplified: nếu maxStreak >= 3 thì thưởng ComboBonusPoints
        // Level config sẽ được truyền qua query — giả sử bonus threshold = 3
        if (maxStreak >= 3)
            bonusPoints = 50; // Default ComboBonusPoints

        bool isPassed = score >= (levelInfo?.PassScore ?? 80);

        // Build ArenaAttempt để update
        var attempt = new ArenaAttempt
        {
            Id = (int)command.AttemptId,
            UserId = userId,
            LevelId = levelInfo?.LevelId ?? 0,
            Score = score,
            TotalCorrect = totalCorrect,
            MaxStreak = maxStreak,
            DurationSeconds = command.DurationSeconds,
            IsPassed = isPassed,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var ans in attemptAnswers) attempt.AddAnswer(ans);

        await _repo.SaveAttemptAsync(attempt, cancellationToken);

        // Upsert progress
        var progress = new ArenaUserProgress
        {
            UserId = userId,
            LevelId = attempt.LevelId,
            IsPassed = isPassed,
            BestScore = score,
            MaxStreak = maxStreak,
            PassedAt = isPassed ? DateTime.UtcNow : null,
            UpdatedAt = DateTime.UtcNow
        };

        // Check achievements
        var newAchievements = ResolveNewAchievements(score, maxStreak, isPassed);

        await _repo.UpsertProgressAndAchievementsAsync(progress, newAchievements, cancellationToken);

        return new ArenaSubmitResultDto
        {
            Score = score,
            TotalCorrect = totalCorrect,
            TotalQuestions = totalQuestions,
            MaxStreak = maxStreak,
            BonusPoints = bonusPoints,
            IsPassed = isPassed,
            IsNewBestScore = true, // Simplified — infra có thể trả về giá trị thực
            IsFirstPass = isPassed,
            UnlockedAchievements = newAchievements.Select(a => a.AchievementId.ToString()).ToList(),
            Message = isPassed ? "Chúc mừng! Bạn đã vượt qua level này." : "Chưa đạt. Hãy thử lại nhé!"
        };
    }

    private Task<Dictionary<long, long?>> GetCorrectOptionMapAsync(long attemptId, CancellationToken cancellationToken)
    {
        // Placeholder: thực tế sẽ inject thêm 1 method vào IArenaQueries
        // hoặc implement trực tiếp trong repo. Để handler clean, inject qua _queries.
        return Task.FromResult(new Dictionary<long, long?>());
    }

    private static List<ArenaUserAchievement> ResolveNewAchievements(int score, int maxStreak, bool isPassed)
    {
        var result = new List<ArenaUserAchievement>();
        // Logic unlock achievement — có thể mở rộng theo business rule
        // Ví dụ: perfect score → achievement ID 1
        if (score == 100)
            result.Add(new ArenaUserAchievement { AchievementId = 1, UnlockedAt = DateTime.UtcNow });
        if (maxStreak >= 10)
            result.Add(new ArenaUserAchievement { AchievementId = 2, UnlockedAt = DateTime.UtcNow });

        return result;
    }
}
