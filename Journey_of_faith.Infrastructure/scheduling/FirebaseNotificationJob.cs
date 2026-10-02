using System.Text.Json;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.notifications;
using Journey_of_faith.Domain.entities.events;
using Journey_of_faith.Infrastructure.context;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Journey_of_faith.Infrastructure.scheduling;

[DisallowConcurrentExecution]
public sealed class FirebaseNotificationJob(
    IFirebaseNotification notification,
    ApplicationDbContext dbContext,
    ILogger<FirebaseNotificationJob> logger) : IJob
{
    public const string PayloadKey = "notification";

    public async Task Execute(IJobExecutionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        try
        {
            var userId = context.MergedJobDataMap.GetString("user_id");
            var payload = context.MergedJobDataMap.GetString(PayloadKey)
                ?? throw new InvalidOperationException("Scheduled notification payload is missing.");
            
            var request = JsonSerializer.Deserialize<ScheduleNotificationRequest>(payload)
                ?? throw new InvalidOperationException("Scheduled notification payload is invalid.");

            var messageId = !string.IsNullOrWhiteSpace(request.Token)
                ? await notification.SendNotificationAsync(request.Token, request.Title, request.Body, request.Data)
                : await notification.SendToTopicAsync(request.Topic!, request.Title, request.Body, request.Data);

            await dbContext.AddAsync(new NotificationLogs
            {
                Title = request.Title,
                Body = request.Body,
                UserId = Guid.Parse(userId),
                SendAt = DateTime.UtcNow,
            });
            context.Result = messageId;
            logger.LogInformation("Firebase notification job {JobKey} accepted with message ID {MessageId}",
                context.JobDetail.Key, messageId);
        }
        catch (Exception ex)
        {
            // Do not log device tokens or payloads. No immediate retries: an ambiguous
            // FCM timeout may already have sent the notification.
            logger.LogError("Firebase notification job {JobKey} failed ({ErrorType})",
                context.JobDetail.Key, ex.GetType().Name);
            throw new JobExecutionException("Firebase notification delivery failed.", ex, refireImmediately: false);
        }
    }
}
