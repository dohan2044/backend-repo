using Journey_of_faith.Application.common.dtos.arena;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using MediatR;

namespace Journey_of_faith.Application.usecases.arena.queries;

// ─── Get All Levels ───────────────────────────────────────────────────────────
public class GetArenaLevelsQuery : IRequest<IReadOnlyList<ArenaLevelSummaryDto>>;

public class GetArenaLevelsHandler : IRequestHandler<GetArenaLevelsQuery, IReadOnlyList<ArenaLevelSummaryDto>>
{
    private readonly IArenaQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public GetArenaLevelsHandler(IArenaQueries queries, ICurrentUserService currentUser)
    {
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ArenaLevelSummaryDto>> Handle(GetArenaLevelsQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");

        return await _queries.GetLevelsAsync(userId, cancellationToken);
    }
}

// ─── Get Level Detail ─────────────────────────────────────────────────────────
public class GetArenaLevelDetailQuery : IRequest<ArenaLevelDetailDto>
{
    public int LevelId { get; set; }
}

public class GetArenaLevelDetailHandler : IRequestHandler<GetArenaLevelDetailQuery, ArenaLevelDetailDto>
{
    private readonly IArenaQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public GetArenaLevelDetailHandler(IArenaQueries queries, ICurrentUserService currentUser)
    {
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<ArenaLevelDetailDto> Handle(GetArenaLevelDetailQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");

        var level = await _queries.GetLevelDetailAsync(request.LevelId, userId, cancellationToken);
        if (level is null)
            throw new NotFoundException("Level không tồn tại.");

        return level;
    }
}

// ─── Get User Progress ────────────────────────────────────────────────────────
public class GetArenaUserProgressQuery : IRequest<IReadOnlyList<ArenaUserProgressDto>>;

public class GetArenaUserProgressHandler : IRequestHandler<GetArenaUserProgressQuery, IReadOnlyList<ArenaUserProgressDto>>
{
    private readonly IArenaQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public GetArenaUserProgressHandler(IArenaQueries queries, ICurrentUserService currentUser)
    {
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ArenaUserProgressDto>> Handle(GetArenaUserProgressQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");

        return await _queries.GetUserProgressAsync(userId, cancellationToken);
    }
}

// ─── Get Attempt History ──────────────────────────────────────────────────────
public class GetArenaAttemptHistoryQuery : IRequest<IReadOnlyList<ArenaAttemptHistoryDto>>;

public class GetArenaAttemptHistoryHandler : IRequestHandler<GetArenaAttemptHistoryQuery, IReadOnlyList<ArenaAttemptHistoryDto>>
{
    private readonly IArenaQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public GetArenaAttemptHistoryHandler(IArenaQueries queries, ICurrentUserService currentUser)
    {
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ArenaAttemptHistoryDto>> Handle(GetArenaAttemptHistoryQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");

        return await _queries.GetAttemptHistoryAsync(userId, cancellationToken);
    }
}

// ─── Get Leaderboard ──────────────────────────────────────────────────────────
public class GetArenaLeaderboardQuery : IRequest<IReadOnlyList<ArenaLeaderboardEntryDto>>
{
    public int LevelId { get; set; }
    public int Top { get; set; } = 20;
}

public class GetArenaLeaderboardHandler : IRequestHandler<GetArenaLeaderboardQuery, IReadOnlyList<ArenaLeaderboardEntryDto>>
{
    private readonly IArenaQueries _queries;

    public GetArenaLeaderboardHandler(IArenaQueries queries) => _queries = queries;

    public async Task<IReadOnlyList<ArenaLeaderboardEntryDto>> Handle(GetArenaLeaderboardQuery request, CancellationToken cancellationToken)
        => await _queries.GetLeaderboardAsync(request.LevelId, request.Top, cancellationToken);
}

// ─── Get Achievements ─────────────────────────────────────────────────────────
public class GetArenaUserAchievementsQuery : IRequest<IReadOnlyList<ArenaAchievementDto>>;

public class GetArenaUserAchievementsHandler : IRequestHandler<GetArenaUserAchievementsQuery, IReadOnlyList<ArenaAchievementDto>>
{
    private readonly IArenaQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public GetArenaUserAchievementsHandler(IArenaQueries queries, ICurrentUserService currentUser)
    {
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ArenaAchievementDto>> Handle(GetArenaUserAchievementsQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");

        return await _queries.GetUserAchievementsAsync(userId, cancellationToken);
    }
}
