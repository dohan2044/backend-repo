using Journey_of_faith.Application.common.dtos.arena;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.compete;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.arena.commands;

// ─── Start Attempt ────────────────────────────────────────────────────────────
public class StartArenaAttemptCommand : IRequest<ArenaStartAttemptDto>
{
    public int LevelId { get; set; }
}

public class StartArenaAttemptHandler : IRequestHandler<StartArenaAttemptCommand, ArenaStartAttemptDto>
{
    private readonly IArenaRepository _repo;
    private readonly IArenaQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public StartArenaAttemptHandler(IArenaRepository repo, IArenaQueries queries, ICurrentUserService currentUser)
    {
        _repo = repo;
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<ArenaStartAttemptDto> Handle(StartArenaAttemptCommand request, CancellationToken cancellationToken)
    {
        if (!int.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");

        var levelDetail = await _queries.GetLevelDetailAsync(request.LevelId, userId, cancellationToken);
        if (levelDetail is null)
            throw new NotFoundException("Level không tồn tại.");

        if (!levelDetail.IsUnlocked)
            throw new ForbiddenException("Bạn chưa đủ điều kiện để thi level này. Hãy hoàn thành level tiên quyết trước.");

        // Tạo attempt mới
        var attempt = new ArenaAttempt
        {
            UserId = userId,
            LevelId = request.LevelId,
            Score = 0,
            TotalCorrect = 0,
            MaxStreak = 0,
            DurationSeconds = 0,
            IsPassed = false,
            CreatedAt = DateTime.UtcNow
        };

        var attemptId = await _repo.SaveAttemptAsync(attempt, cancellationToken);

        // Trả câu hỏi (không có IsCorrect)
        var result = await _queries.GetAttemptQuestionsAsync(attemptId, userId, cancellationToken);
        if (result is null)
            throw new NotFoundException("Không tìm thấy câu hỏi cho level này.");

        return result;
    }
}
