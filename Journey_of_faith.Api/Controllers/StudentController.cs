using Asp.Versioning;
using Journey_of_faith.Api.Attributes;
using Journey_of_faith.Application.usecases.students.commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.Controllers;

[ApiController]
[Route("api/{version:apiVersion}/students")]
[ApiVersion(1)]
public class StudentController(IMediator mediator) : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpPost("groups")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupStudent command)
    {
        await mediator.Send(command);
        return CreatedAtAction(nameof(CreateGroup), new { version = "1", messge = "Tạo nhóm thanh công"});
    }

}