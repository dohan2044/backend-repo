using System.Net.Mime;
using Asp.Versioning;
using Journey_of_faith.Api.Attributes;
using Journey_of_faith.Api.authorization;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.common.dtos.arena;
using Journey_of_faith.Application.usecases.arena.commands;
using Journey_of_faith.Application.usecases.arena.queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("api/v{version:apiVersion}/arena")]
public class ArenaController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArenaController(IMediator mediator) => _mediator = mediator;

    // ─── Queries ──────────────────────────────────────────────────────────────

    /// <summary>Danh sách levels kèm trạng thái của user hiện tại</summary>
    [HttpGet("levels")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ArenaLevelSummaryDto>>), StatusCodes.Status200OK)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> GetLevels()
    {
        var data = await _mediator.Send(new GetArenaLevelsQuery());
        return Ok(new ApiResponse<IReadOnlyList<ArenaLevelSummaryDto>>
        {
            Message = "Lấy danh sách levels thành công.",
            Data = data
        });
    }

    /// <summary>Chi tiết 1 level</summary>
    [HttpGet("levels/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ArenaLevelDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> GetLevelDetail(int id)
    {
        var data = await _mediator.Send(new GetArenaLevelDetailQuery { LevelId = id });
        return Ok(new ApiResponse<ArenaLevelDetailDto>
        {
            Message = "Lấy chi tiết level thành công.",
            Data = data
        });
    }

    /// <summary>Tiến độ user qua các levels</summary>
    [HttpGet("progress")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ArenaUserProgressDto>>), StatusCodes.Status200OK)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> GetProgress()
    {
        var data = await _mediator.Send(new GetArenaUserProgressQuery());
        return Ok(new ApiResponse<IReadOnlyList<ArenaUserProgressDto>>
        {
            Message = "Lấy tiến độ thành công.",
            Data = data
        });
    }

    /// <summary>Lịch sử lần thi của user</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ArenaAttemptHistoryDto>>), StatusCodes.Status200OK)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> GetHistory()
    {
        var data = await _mediator.Send(new GetArenaAttemptHistoryQuery());
        return Ok(new ApiResponse<IReadOnlyList<ArenaAttemptHistoryDto>>
        {
            Message = "Lấy lịch sử thi thành công.",
            Data = data
        });
    }

    /// <summary>Bảng xếp hạng theo level</summary>
    [HttpGet("levels/{id:int}/leaderboard")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ArenaLeaderboardEntryDto>>), StatusCodes.Status200OK)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> GetLeaderboard(int id, [FromQuery] int top = 20)
    {
        var data = await _mediator.Send(new GetArenaLeaderboardQuery { LevelId = id, Top = top });
        return Ok(new ApiResponse<IReadOnlyList<ArenaLeaderboardEntryDto>>
        {
            Message = "Lấy bảng xếp hạng thành công.",
            Data = data
        });
    }

    /// <summary>Thành tựu của user</summary>
    [HttpGet("achievements")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ArenaAchievementDto>>), StatusCodes.Status200OK)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> GetAchievements()
    {
        var data = await _mediator.Send(new GetArenaUserAchievementsQuery());
        return Ok(new ApiResponse<IReadOnlyList<ArenaAchievementDto>>
        {
            Message = "Lấy thành tựu thành công.",
            Data = data
        });
    }

    // ─── Commands ─────────────────────────────────────────────────────────────

    /// <summary>Bắt đầu 1 lần thi mới — trả câu hỏi không có đáp án đúng</summary>
    [HttpPost("levels/{id:int}/start")]
    [ProducesResponseType(typeof(ApiResponse<ArenaStartAttemptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> StartAttempt(int id)
    {
        var data = await _mediator.Send(new StartArenaAttemptCommand { LevelId = id });
        return Ok(new ApiResponse<ArenaStartAttemptDto>
        {
            Message = "Bắt đầu thi thành công.",
            Data = data
        });
    }

    /// <summary>Nộp bài thi — tính điểm, cập nhật tiến độ, unlock thành tựu</summary>
    [HttpPost("attempts/{attemptId:long}/submit")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<ArenaSubmitResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> SubmitAttempt(long attemptId, [FromBody] SubmitArenaAttemptRequest body)
    {
        var data = await _mediator.Send(new SubmitArenaAttemptCommand
        {
            AttemptId = attemptId,
            Answers = body.Answers.Select(a => new SubmitArenaAttemptCommand.AnswerItem(a.QuestionId, a.SelectedOptionId)).ToList(),
            DurationSeconds = body.DurationSeconds
        });
        return Ok(new ApiResponse<ArenaSubmitResultDto>
        {
            Message = data.Message,
            Data = data
        });
    }

    // ─── Admin ────────────────────────────────────────────────────────────────

    /// <summary>[Admin] Tạo level mới</summary>
    [HttpPost("levels")]
    // [HasPermission(Permissions.Arena.MANAGE)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> CreateLevel([FromBody] CreateArenaLevelCommand command)
    {
        var id = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo level thành công.", Data = new { id } });
    }

    /// <summary>[Admin] Cập nhật level</summary>
    [HttpPut("levels/{id:int}")]
    // [HasPermission(Permissions.Arena.MANAGE)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> UpdateLevel(int id, [FromBody] UpdateArenaLevelCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return Ok(new { Message = "Cập nhật level thành công." });
    }

    /// <summary>[Admin] Xóa level (soft delete)</summary>
    [HttpDelete("levels/{id:int}")]
    // [HasPermission(Permissions.Arena.MANAGE)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> DeleteLevel(int id)
    {
        await _mediator.Send(new DeleteArenaLevelCommand { LevelId = id });
        return NoContent();
    }

    /// <summary>[Admin] Thêm câu hỏi vào level</summary>
    [HttpPost("levels/{id:int}/questions")]
    // [HasPermission(Permissions.Arena.MANAGE)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [MapToApiVersion(1)]
    public async Task<IActionResult> AddQuestion(int id, [FromBody] AddArenaQuestionCommand command)
    {
        command.LevelId = id;
        var questionId = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new { Message = "Thêm câu hỏi thành công.", Data = new { questionId } });
    }
}

// Request DTO dùng riêng cho controller (không expose qua command trực tiếp)
public class SubmitArenaAttemptRequest
{
    public List<AnswerItem> Answers { get; set; } = new();
    public int DurationSeconds { get; set; }

    public record AnswerItem(long QuestionId, long? SelectedOptionId);
}
