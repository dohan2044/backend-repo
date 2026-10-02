using Asp.Versioning;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.usecases.churchs.commands;
using Journey_of_faith.Application.usecases.churchs.dtos;
using Journey_of_faith.Application.usecases.churchs.queries;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities.catholic;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Mime;
using System.IO;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Infrastructure.repositories;
using Journey_of_faith.Application.common.dtos.church; // Đảm bảo đã có using System.IO;

namespace Journey_of_faith.Api.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{version:apiVersion}/churches")]

    public class ChurchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChurchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateChurch(
            [FromForm] CreateChurchCommand command)
        {
            var churchId = await _mediator.Send(command);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<int>
                {
                    Message = "Tạo nhà thờ thành công.",
                    Data = churchId
                });
        }

        [MapToApiVersion(1)]
        [HttpPost("mass")]
        [Authorize]
        public async Task<IActionResult> CreateMassAndLiturgy([FromBody] CreateMassAndLiturgyCommand command)
        {
            await _mediator.Send(command);
            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<bool>
                {
                    Message = "Tạo lịch thành công",
                    Data = true
                }
            );
        }
        [MapToApiVersion(1)]
        [HttpGet("massSchedule-today")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMassScheduleToday([FromQuery] bool nextDay = false, [FromQuery] string? province = null)
        {
            var result = await _mediator.Send(new GetMassScheduleTodayQuery
            {
                NextDay = nextDay,
                Province = province
            });
            return Ok(new ApiResponse<IReadOnlyList<MassScheduleTodayDto>>
            {
                Data = result,
                Message = "Lấy dữ liệu thành công"
            });
        }
        [MapToApiVersion(1)]
        [HttpPost("liturgy")]
        [Authorize]
        public async Task<IActionResult> CreateLiturgy([FromBody] CreateLiturgy command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<bool>
                {
                    Message = "Tạo phụng vụ thành công",
                    Data = result
                }
            );
        }
        [HttpGet("liturgy-today")]
        [MapToApiVersion(1)]
        [AllowAnonymous]
        public async Task<IActionResult> GetLiturgyToday()
        {
            var result = await _mediator.Send(new GetLiturgyTodayQuery());
            return Ok(new ApiResponse<LiturgyViewDto?>
            {
                Data = result,
                Message = "Lấy bài phụng vụ thành công"
            });
        }
        [MapToApiVersion(1)]
        [HttpPost("dioceses")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize]
        public async Task<IActionResult> CreateDiocese(
            [FromForm] CreateDioceseCommand command,
            IFormFile? file)
        {
            if (file != null)
            {
                // SỬA: tương tự
                var path = System.IO.Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "uploads",
                    "dioceses");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var uniqueFile =
                    $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";

                var fullPath =
                    System.IO.Path.Combine(path, uniqueFile);

                using (var stream = new FileStream(fullPath, System.IO.FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                command.Thumbnail =
                    System.IO.Path.Combine(
                        "uploads",
                        "dioceses",
                        uniqueFile);
            }

            var dioceseId = await _mediator.Send(command);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<int>
                {
                    Message = "Tạo giáo phận thành công.",
                    Data = dioceseId
                });
        }
        [MapToApiVersion(1)]
        [HttpGet("dioceses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetDiocese()
        {
            var result =
                await _mediator.Send(new GetDioceseQuery());

            return Ok(
                new ApiResponse<IEnumerable<DioceseViewDto>>
                {
                    Message = "Lấy dữ liệu thành công.",
                    Data = result
                });
        }

        [MapToApiVersion(1)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> SearchChurches(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? search)
        {
            var result =
                await _mediator.Send(
                    new GetChurchWithMassScheduleQueries
                    {
                        Page = page,
                        PageSize = pageSize,
                        Search = search
                    });

            return Ok(result);
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        public async Task<IActionResult> UpdateChurch(
            [FromForm] UpdateChurchCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> DeleteChurch(
            [FromRoute] int id,
            [FromQuery] bool? force)
        {
            var result =
                await _mediator.Send(
                    new DeleteChurchCommand
                    {
                        Id = id,
                        Force = force
                    });

            return Ok(result);
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetChurchDetails(
            [FromRoute] int id)
        {
            var church =
                await _mediator.Send(
                    new GetChurchDetailsQuery
                    {
                        Id = id
                    });

            if (church is null)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Message = "Không tìm thấy nhà thờ.",
                        Data = new { Id = id }
                    });
            }

            return Ok(
                new ApiResponse<ChurchViewDto>
                {
                    Message = "Lấy chi tiết nhà thờ thành công.",
                    Data = church
                });
        }

        [HttpGet("condition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetCondition(
            [FromQuery] string? churchName,
            [FromQuery] string? province,
            [FromQuery] string? ward,
            [FromQuery] string? time,
            [FromQuery] int? page,
            [FromQuery] int? pageSize)
        {
            var result = await _mediator.Send(
                new GetChurchWithCondition
                {
                    NameChurch = churchName,
                    Province = province,
                    Ward = ward,
                    Time = time,
                    Page = page.HasValue ? page.Value : 1,
                    PageSize = pageSize.HasValue ? pageSize.Value : 10
                }
            );

            return Ok(new ApiResponse<PagedResult<ChurchViewDto>>
            {
                Data = result,
                Message = "Lấy dữ liệu thành công"
            });
        }

        [HttpPost("{churchId:int}/follow")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FollowChurch([FromRoute] int churchId)
        {
            await _mediator.Send(new FollowChurchCommand { ChurchId = churchId });
            return Ok(new ApiResponse<object>
            {
                Message = "Đã thêm nhà thờ vào danh sách theo dõi.",
                Data = new { ChurchId = churchId }
            });
        }
        [MapToApiVersion(1)]
        [HttpDelete("{churchId:int}/unfollow")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UnfollowChurch([FromRoute] int churchId)
        {
            await _mediator.Send(new UnfollowChurchCommand { ChurchId = churchId });
            return Ok(new ApiResponse<object>
            {
                Message = "Đã hủy theo dõi nhà thờ.",
                Data = new { ChurchId = churchId }
            });
        }

        [HttpGet("following")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollowingChurches()
        {
            var result =
                await _mediator.Send(
                    new GetFollowedChurchesQuery());

            return Ok(
                new ApiResponse<IEnumerable<ChurchViewDto>>
                {
                    Message = result.Any()
                        ? "Lấy danh sách nhà thờ theo dõi thành công."
                        : "Bạn chưa theo dõi nhà thờ nào.",
                    Data = result
                });
        }
        // [MapToApiVersion(1)]
        // [HttpGet("mass-schedules/personalized")]
        // [Authorize]
        // [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetPersonalizedMassSchedules(
        //     [FromQuery] GetPersonalizedMassSchedulesQuery query)
        // {
        //     var result =
        //         await _mediator.Send(query);

        //     return Ok(
        //         new ApiResponse<IEnumerable<PersonalizedMassScheduleItemDto>>
        //         {
        //             Message = result.Any()
        //                 ? "Lấy lịch lễ cá nhân hóa thành công."
        //                 : "Không có lịch lễ trong phạm vi lọc hoặc bạn chưa theo dõi nhà thờ nào.",
        //             Data = result
        //         });
        // }
        [MapToApiVersion(1)]
        [HttpGet("reminder-setting")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReminderSetting()
        {
            var result = await _mediator.Send(new GetMassReminderSettingQuery());
            return Ok(new ApiResponse<ReminderSettingDto>
            {
                Message = "Lấy cấu hình nhắc lễ thành công.",
                Data = result
            });
        }

        [HttpPut("reminder-setting")]
        [Authorize]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateReminderSetting(
            [FromBody] UpdateMassReminderSettingCommand command)
        {
            var result =
                await _mediator.Send(command);



            return Ok(
                new ApiResponse<ReminderSettingDto>
                {
                    Message =
                        "Cập nhật cấu hình nhắc lễ thành công.",
                    Data = result
                });
        }

        [MapToApiVersion(1)]
        [Authorize]
        [HttpPost("dailyWords")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateDailyWord(
            [FromBody] CreateDailyWordCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created,
                new ApiResponse<object>
                {
                    Message = "Tạo lời Chúa hằng ngày thành công.",
                    Data = new { Id = result }
                });
        }

        [HttpGet("dailyWords/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetDailyWord(
            [FromQuery] GetDailyWordCommand command)
        {
            var daily = await _mediator.Send(command);
            if (daily is null)
            {
                return Ok(new ApiResponse<string>
                {
                    Message = "Không tìm thấy lời Chúa.",
                    Data = null
                });
            }
            return Ok(new ApiResponse<DailyWordViewDto>
            {
                Message = "Lấy lời Chúa thành công.",
                Data = daily
            });
        }
    }
}
