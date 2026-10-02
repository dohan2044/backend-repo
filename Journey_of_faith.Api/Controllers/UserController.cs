using System.Net.Mime;
using Asp.Versioning;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Application.usecases.users.commands;
using Journey_of_faith.Application.usecases.users.queries;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Journey_of_faith.Api.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [MapToApiVersion(1)]    
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            await _mediator.Send(command);
            return StatusCode(statusCode: StatusCodes.Status201Created, new
            {
                message = "đăng ký thành công",
                success = true
            });
        }
        [HttpGet]
        // [Consumes(MediaTypeNames.Application.j)]
        // [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetUsers([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetUsersQuery { Page = page, PageSize = pageSize, Search = search});
            return Ok(new ApiResponse<PagedResult<UserResponseDto>>
            {
                Data = result,
                Message = "Lấy dữ liệu thành công."
            });
        }
        [HttpGet("me")]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetMe()
        {
            var user = await _mediator.Send(new GetMeQuery());
            return Ok(new ApiResponse<UserResponseDto>
            {
                Data  = user,
                Message = "Laays nguoi dung thanh cong."
            });
        }
        [HttpDelete("{UsreId}")]
        [MapToApiVersion(1)]
        public async Task<IActionResult> DeleteUser([FromRoute] string UsreId)
        {
            var isDeleted = await _mediator.Send(new DeleteUserCommand{Id = UsreId});

            return Ok(isDeleted);
        }
        [HttpPost("increment")]
        [MapToApiVersion(1)]
        public async Task<IActionResult> Increment([FromBody] IncrementCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}
