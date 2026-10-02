using Asp.Versioning;
using FirebaseAdmin.Messaging;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.notifications;
using Journey_of_faith.Application.usecases.notifications.queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("api/v{version:apiVersion}/notifications")]
public class NotificationController : ControllerBase
{
    private readonly IMediator mediator;
    public NotificationController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    [MapToApiVersion(1)]
    [HttpPost("send-notification")]
    public async Task<IActionResult> PushToUser(string? token, string? topic, string title, string message, Guid? userId)
    {
        if (!string.IsNullOrWhiteSpace(token) && (userId is null || userId == Guid.Empty))
        {
            return BadRequest(new
            {
                Success = false,
                Error = "userId hợp lệ là bắt buộc khi gửi thông báo theo device token."
            });
        }

        try
        {
            var dataPayload = new Dictionary<string, string> { { "click_action", "OPEN_ARTICLE" }, { "articleId", "123" } };
            var messageId = await mediator.Send(new PushNotificationCommand
            {
                Topic = topic,
                Token = token,
                UserId = userId,
                Title = title,
                Body = message,
                Data = dataPayload
            });

            return Ok(new {Success = true, MessageId = messageId});
        } catch (FirebaseMessagingException ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message, ErrorCode = ex.MessagingErrorCode.ToString() });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [MapToApiVersion(1)]
    [HttpPost("fcm-register")]
    public async Task<IActionResult> FCMRegister([FromBody] FcmRegisterCommand command)
    {
        await mediator.Send(command);
        return Ok(new {Success = true});
    }


    [MapToApiVersion(1)]
    [HttpPost("subscribe-topic")]
    public async Task<IActionResult> SubscribeToTopic([FromBody] SubscribeToTopicCommand command)
    {
        try
        {
            var response = await mediator.Send(command);
            return Ok(new { Success = true, response.SuccessCount, response.FailureCount, response.Errors });
        }
        catch (FirebaseMessagingException ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message, ErrorCode = ex.MessagingErrorCode.ToString() });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [MapToApiVersion(1)]
    [HttpPost("unsubscribe-topic")]
    public async Task<IActionResult> UnsubscribeFromTopic([FromBody] UnsubscribeFromTopicCommand command)
    {
        try
        {
            var response = await mediator.Send(command);
            return Ok(new { Success = true, response.SuccessCount, response.FailureCount, response.Errors });
        }
        catch (FirebaseMessagingException ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message, ErrorCode = ex.MessagingErrorCode.ToString() });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [MapToApiVersion(1)]
    [HttpGet("users/devices")]
    public async Task<IActionResult> GetAllUserDeviceTokens(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllUserDeviceTokensQuery(), cancellationToken);
        return Ok(result);
    }

    [MapToApiVersion(1)]
    [HttpGet("users/{userId:guid}/devices")]
    public async Task<IActionResult> GetUserWithDevices([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserWithDevicesQuery(userId), cancellationToken);
        return result is null ? NotFound(new { Success = false, Error = "Không tìm thấy người dùng." }) : Ok(result);
    }
}
