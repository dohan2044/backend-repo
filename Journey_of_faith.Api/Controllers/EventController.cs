using Asp.Versioning;
using Journey_of_faith.Api.Attributes;
using Journey_of_faith.Api.authorization;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.usecases.events.commands;
using Journey_of_faith.Application.usecases.events.queries;
using Journey_of_faith.Application.common.dtos.Events;
using Journey_of_faith.Domain.dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Journey_of_faith.Api.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Authorize] // validate đăng nhập
    [Route("api/v{version:apiVersion}/events")]
    public class EventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [MapToApiVersion(1)]
        [HttpPost("category")]
        [HasPermission(Permissions.Event.CREATE_CATEGORY)] // check quyền
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateEventCategoryCommand command)
        {
            var categoryId = await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new ApiResponse<int>
            {
                Message = "Tạo danh mục sự kiện thành công.",
                Data = categoryId
            });
        }
        [MapToApiVersion(1)]
        [HttpGet("category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _mediator.Send(new GetEventCategoriesQuery());
            return Ok(new ApiResponse<IEnumerable<EventCategoryDto>>
            {
                Message = categories.Any() ? "Lấy danh mục sự kiện thành công." : "Không có danh mục sự kiện nào.",
                Data = categories
            });
        }
        [MapToApiVersion(1)]
        [HttpPost]
        [HasPermission(Permissions.Event.CREATE)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            var eventId = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<int>
            {
                Message = "Tạo sự kiện thành công.",
                Data = eventId
            });
        }
        [MapToApiVersion(1)]
        [HttpPut("{id:int}")]
        [HasPermission(Permissions.Event.UPDATE)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEvent([FromRoute] int id, [FromBody] UpdateEventCommand command)
        {
            command.Id = id;
            var updated = await _mediator.Send(command);
            return Ok(new ApiResponse<object>
            {
                Message = updated ? "Cập nhật sự kiện thành công." : "Không thể cập nhật sự kiện.",
                Data = new { Id = id }
            });
        }
        [MapToApiVersion(1)]
        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.Event.DELETE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            var deleted = await _mediator.Send(new DeleteEventCommand { Id = id });
            return Ok(new ApiResponse<object>
            {
                Message = deleted ? "Xóa sự kiện thành công." : "Không thể xóa sự kiện.",
                Data = new { Id = id }
            });
        }
        [MapToApiVersion(1)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents([FromQuery] GetEventsQuery query)
        {
            var events = await _mediator.Send(query);
            return Ok(new ApiResponse<PagedResult<EventViewDto>>
            {
                Message = events.TotalCount > 0 ? "Lấy danh sách sự kiện thành công." : "Không có sự kiện phù hợp.",
                Data = events
            });
        }
        [MapToApiVersion(1)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            var details = await _mediator.Send(new GetEventDetailsQuery { EventId = id });
            if (details is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không tìm thấy sự kiện.",
                    Data = new { Id = id }
                });
            }

            return Ok(new ApiResponse<EventDetailsDto>
            {
                Message = "Lấy chi tiết sự kiện thành công.",
                Data = details
            });
        }
        [MapToApiVersion(1)]
        [HttpPost("{id:int}/follow")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FollowEvent([FromRoute] int id)
        {
            await _mediator.Send(new FollowEventCommand { EventId = id });
            return Ok(new ApiResponse<object>
            {
                Message = "Theo dõi sự kiện thành công.",
                Data = new { EventId = id }
            });
        }
        [MapToApiVersion(1)]
        [HttpDelete("{id:int}/unfollow")]
        [HasPermission(Permissions.Event.USER_DELETE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UnfollowEvent([FromRoute] int id)
        {
            await _mediator.Send(new UnfollowEventCommand { EventId = id });
            return Ok(new ApiResponse<object>
            {
                Message = "Hủy theo dõi sự kiện thành công.",
                Data = new { EventId = id }
            });
        }
        [MapToApiVersion(1)]
        [HttpGet("following")]
        [HasPermission(Permissions.Event.VIEW)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollowedEvents([FromQuery] GetFollowedEventsQuery query)
        {
            var events = await _mediator.Send(query);
            return Ok(new ApiResponse<IEnumerable<EventViewDto>>
            {
                Message = events.Any() ? "Lấy danh sách sự kiện theo dõi thành công." : "Bạn chưa theo dõi sự kiện nào.",
                Data = events
            });
        }


        [MapToApiVersion(1)]
        [HttpPost("comment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateComment([FromBody] CreateEventComment comment)
        {
            var result = await _mediator.Send(comment);
            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<bool>
                {
                    Message = "Tạo Comment thành công"
                }
            );
        }

        [MapToApiVersion(1)]
        [HttpGet("{Id}/comment")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComment([FromRoute]int Id)
        {
            var result = await _mediator.Send(new GetCommentForEventQuery {EventId = Id});
            return Ok(new ApiResponse<IReadOnlyList<EventCommentDto>>
            {
                Message = "Lấy comment thành công",
                Data = result
            });
        } 
    }
}
