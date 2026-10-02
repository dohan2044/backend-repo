using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.compete;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.arena.commands;

// ─── Create Level ─────────────────────────────────────────────────────────────
public class CreateArenaLevelCommand : IRequest<int>
{
    public string LevelCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Tag { get; set; }
    public string? Description { get; set; }
    public string? FocusTopics { get; set; }
    public int TotalQuestions { get; set; } = 10;
    public int TimePerQuestion { get; set; } = 15;
    public int PassScore { get; set; } = 8;
    public int ComboBonusPoints { get; set; } = 50;
    public decimal ComboMultiplier { get; set; } = 2.0m;
    public int? PrerequisiteLevelId { get; set; }
}

public class CreateArenaLevelHandler : IRequestHandler<CreateArenaLevelCommand, int>
{
    private readonly IArenaRepository _repo;
    private readonly IArenaQueries _queries;

    public CreateArenaLevelHandler(IArenaRepository repo, IArenaQueries queries)
    {
        _repo = repo;
        _queries = queries;
    }

    public async Task<int> Handle(CreateArenaLevelCommand command, CancellationToken cancellationToken)
    {
        if (await _queries.LevelCodeExistsAsync(command.LevelCode, cancellationToken))
            throw new ConfictException($"LevelCode '{command.LevelCode}' đã tồn tại.");

        // if (command.PrerequisiteLevelId.HasValue &&
        //     !await _queries.LevelExistsAsync(command.PrerequisiteLevelId.Value, cancellationToken))
        //     throw new NotFoundException("Level tiên quyết không tồn tại.");

        var level = new ArenaLevels
        {
            LevelCode = command.LevelCode,
            Title = command.Title,
            Subtitle = command.Subtitle,
            Tag = command.Tag,
            Description = command.Description,
            FocusTopics = command.FocusTopics,
            TotalQuestions = command.TotalQuestions,
            TimePerQuestion = command.TimePerQuestion,
            PassScore = command.PassScore,
            ComboBonusPoints = command.ComboBonusPoints,
            ComboMultiplier = command.ComboMultiplier,
            PrerequisiteLevelId = command.PrerequisiteLevelId
        };

        return await _repo.CreateLevelAsync(level, cancellationToken);
    }
}

// ─── Update Level ─────────────────────────────────────────────────────────────
public class UpdateArenaLevelCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Tag { get; set; }
    public string? Description { get; set; }
    public string? FocusTopics { get; set; }
    public int TotalQuestions { get; set; }
    public int TimePerQuestion { get; set; }
    public int PassScore { get; set; }
    public int ComboBonusPoints { get; set; }
    public decimal ComboMultiplier { get; set; }
    public int? PrerequisiteLevelId { get; set; }
}

public class UpdateArenaLevelHandler : IRequestHandler<UpdateArenaLevelCommand, bool>
{
    private readonly IArenaRepository _repo;
    private readonly IArenaQueries _queries;

    public UpdateArenaLevelHandler(IArenaRepository repo, IArenaQueries queries)
    {
        _repo = repo;
        _queries = queries;
    }

    public async Task<bool> Handle(UpdateArenaLevelCommand command, CancellationToken cancellationToken)
    {
        if (!await _queries.LevelExistsAsync(command.Id, cancellationToken))
            throw new NotFoundException("Level không tồn tại.");

        var level = new ArenaLevels
        {
            Id = command.Id,
            Title = command.Title,
            Subtitle = command.Subtitle,
            Tag = command.Tag,
            Description = command.Description,
            FocusTopics = command.FocusTopics,
            TotalQuestions = command.TotalQuestions,
            TimePerQuestion = command.TimePerQuestion,
            PassScore = command.PassScore,
            ComboBonusPoints = command.ComboBonusPoints,
            ComboMultiplier = command.ComboMultiplier,
            PrerequisiteLevelId = command.PrerequisiteLevelId
        };

        return await _repo.UpdateLevelAsync(level, cancellationToken);
    }
}

// ─── Delete Level ─────────────────────────────────────────────────────────────
public class DeleteArenaLevelCommand : IRequest<bool>
{
    public int LevelId { get; set; }
}

public class DeleteArenaLevelHandler : IRequestHandler<DeleteArenaLevelCommand, bool>
{
    private readonly IArenaRepository _repo;
    private readonly IArenaQueries _queries;

    public DeleteArenaLevelHandler(IArenaRepository repo, IArenaQueries queries)
    {
        _repo = repo;
        _queries = queries;
    }

    public async Task<bool> Handle(DeleteArenaLevelCommand command, CancellationToken cancellationToken)
    {
        if (!await _queries.LevelExistsAsync(command.LevelId, cancellationToken))
            throw new NotFoundException("Level không tồn tại.");

        return await _repo.DeleteLevelAsync(command.LevelId, cancellationToken);
    }
}

// ─── Add Question ─────────────────────────────────────────────────────────────
public class AddArenaQuestionCommand : IRequest<int>
{
    public int LevelId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public List<OptionItem> Options { get; set; } = new();

    public record OptionItem(string Content, bool IsCorrect, short SortOrder);
}

public class AddArenaQuestionHandler : IRequestHandler<AddArenaQuestionCommand, int>
{
    private readonly IArenaRepository _repo;
    private readonly IArenaQueries _queries;

    public AddArenaQuestionHandler(IArenaRepository repo, IArenaQueries queries)
    {
        _repo = repo;
        _queries = queries;
    }

    public async Task<int> Handle(AddArenaQuestionCommand command, CancellationToken cancellationToken)
    {
        if (!await _queries.LevelExistsAsync(command.LevelId, cancellationToken))
            throw new NotFoundException("Level không tồn tại.");

        if (command.Options.Count(o => o.IsCorrect) != 1)
            throw new BadRequestException("Câu hỏi phải có đúng 1 đáp án đúng.");

        var question = new ArenaQuestion
        {
            LevelId = command.LevelId,
            Content = command.Content,
            Explanation = command.Explanation,
            IsActive = true
        };

        foreach (var opt in command.Options)
            question.AddOption(new ArenaQuestionOption
            {
                Content = opt.Content,
                IsCorrect = opt.IsCorrect,
                SortOrder = opt.SortOrder
            });

        return await _repo.AddQuestionAsync(question, cancellationToken);
    }
}
