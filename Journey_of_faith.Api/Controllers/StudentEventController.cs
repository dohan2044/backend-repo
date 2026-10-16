using System.Net.Mime;
using Asp.Versioning;
using Domain.Entities;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.events.commands;
using Journey_of_faith.Application.usecases.events.queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.Controllers;

[ApiVersion(1)]
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/student-events")]
public class StudentEventController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentEvents([FromQuery] GetStudentEventsQuery query)
    {
        var data = await mediator.Send(query);
        return Ok(new ApiResponse<IReadOnlyList<StudentEvent>>
        {
            Message = data.Count > 0 ? "Lấy danh sách sự kiện sinh viên thành công." : "Không có sự kiện nào.",
            Data = data
        });
    }

    [MapToApiVersion(1)]
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateStudentEventCommand command)
    {
        var id = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<int>
        {
            Message = "Tạo sự kiện sinh viên thành công.",
            Data = id
        });
    }

    [MapToApiVersion(1)]
    [HttpPut("{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStudentEventRequest body)
    {
        await mediator.Send(new UpdateStudentEventCommand
        {
            Id = id,
            Title = body.Title,
            Description = body.Description,
            Location = body.Location,
            StartAt = body.StartAt,
            EndAt = body.EndAt,
            MaxParticipants = body.MaxParticipants,
            RegistrationDeadline = body.RegistrationDeadline,
            CoverImageUrl = body.CoverImageUrl
        });
        return Ok(new ApiResponse<object>
        {
            Message = "Cập nhật sự kiện sinh viên thành công.",
            Data = new { Id = id }
        });
    }

    [MapToApiVersion(1)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await mediator.Send(new DeleteStudentEventCommand { Id = id });
        return Ok(new ApiResponse<object>
        {
            Message = "Xóa sự kiện sinh viên thành công.",
            Data = new { Id = id }
        });
    }

    [MapToApiVersion(1)]
    [HttpPost("{id:int}/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromRoute] int id)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId))
        {
            throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
        }

        await mediator.Send(new RegisterStudentEventCommand { EventId = id, UserId = userId });
        return Ok(new ApiResponse<object>
        {
            Message = "Đăng ký sự kiện thành công.",
            Data = new { EventId = id }
        });
    }
}

public class UpdateStudentEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int? MaxParticipants { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public string? CoverImageUrl { get; set; }
}
