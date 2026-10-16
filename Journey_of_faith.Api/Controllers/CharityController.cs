using System.Net.Mime;
using Asp.Versioning;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.charities.commands;
using Journey_of_faith.Application.usecases.charities.queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.Controllers;

[ApiVersion(1)]
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/charities")]
public class CharityController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCharities([FromQuery] GetCharitiesQuery query)
    {
        var data = await mediator.Send(query);
        return Ok(new ApiResponse<object>
        {
            Message = data.Count > 0 ? "Lấy danh sách hoạt động từ thiện thành công." : "Không có hoạt động từ thiện nào.",
            Data = data
        });
    }

    [MapToApiVersion(1)]
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCharityCommand command)
    {
        var id = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<int>
        {
            Message = "Tạo hoạt động từ thiện thành công.",
            Data = id
        });
    }

    [MapToApiVersion(1)]
    [HttpPut("{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCharityRequest body)
    {
        await mediator.Send(new UpdateCharityCommand
        {
            Id = id,
            Title = body.Title,
            Description = body.Description,
            Location = body.Location,
            StartAt = body.StartAt,
            EndAt = body.EndAt,
            MaxParticipants = body.MaxParticipants,
            CoverImageUrl = body.CoverImageUrl
        });
        return Ok(new ApiResponse<object>
        {
            Message = "Cập nhật hoạt động từ thiện thành công.",
            Data = new { Id = id }
        });
    }

    [MapToApiVersion(1)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await mediator.Send(new DeleteCharityCommand { Id = id });
        return Ok(new ApiResponse<object>
        {
            Message = "Xóa hoạt động từ thiện thành công.",
            Data = new { Id = id }
        });
    }

    [MapToApiVersion(1)]
    [HttpPost("{id:int}/register")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromRoute] int id, [FromBody] RegisterCharityRequest? body)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId))
        {
            throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
        }

        await mediator.Send(new RegisterCharityCommand { CharityId = id, UserId = userId, Note = body?.Note });
        return Ok(new ApiResponse<object>
        {
            Message = "Đăng ký tham gia hoạt động từ thiện thành công.",
            Data = new { CharityId = id }
        });
    }
}

public class UpdateCharityRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int? MaxParticipants { get; set; }
    public string? CoverImageUrl { get; set; }
}

public class RegisterCharityRequest
{
    public string? Note { get; set; }
}
