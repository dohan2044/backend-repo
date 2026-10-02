using Asp.Versioning;
using Journey_of_faith.Api.authorization;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.Controllers;

[ApiVersion(1)]
[ApiController]
// [Authorize(Policy = NotificationSchedulingAuthorization.PolicyName)]
[Route("api/v{version:apiVersion}/notifications/schedules")]
public sealed class NotificationSchedulesController(INotificationScheduler scheduler) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ScheduledNotificationResponse>> Create(
        [FromBody] ScheduleNotificationRequest request, CancellationToken cancellationToken)
    {
        var result = await scheduler.ScheduleAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { version = "1", id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScheduledNotificationResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await scheduler.GetAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        return await scheduler.CancelAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
