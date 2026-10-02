using System.Text.Json;
using System.Text.RegularExpressions;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;

namespace Journey_of_faith.Infrastructure.scheduling;

public sealed class QuartzNotificationScheduler(
    ISchedulerFactory schedulerFactory,
    TimeProvider timeProvider,
    ICurrentUserService _currentUser,
    ILogger<QuartzNotificationScheduler> logger) : INotificationScheduler
{
    private const string Group = "firebase-notifications";

    public QuartzNotificationScheduler(ISchedulerFactory schedulerFactory, TimeProvider timeProvider, ICurrentUserService currentUserService)
        : this(schedulerFactory, timeProvider,currentUserService, NullLogger<QuartzNotificationScheduler>.Instance)
    {
    }

    public async Task<ScheduledNotificationResponse> ScheduleAsync(
        ScheduleNotificationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidatePayload(request);
        try
        {
            if(Guid.TryParse(_currentUser.UserId, out Guid Id))
            {
                throw new ArgumentException("", nameof(Id));
            } 
            var now = timeProvider.GetUtcNow();
            var hasCron = !string.IsNullOrWhiteSpace(request.CronExpression);
            if (request.RunAt.HasValue == hasCron)
                throw new BadRequestException("Cung cấp đúng một trong hai: runAt hoặc cronExpression.");

            var id = Guid.NewGuid();
            var jobKey = JobKeyFor(id);
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(TriggerKeyFor(id))
                .ForJob(jobKey);

            if (request.RunAt is { } runAt)
            {
                if (runAt <= now)
                    throw new BadRequestException("runAt phải là thời điểm trong tương lai, kèm múi giờ (Z hoặc +07:00).");

                triggerBuilder.StartAt(runAt.ToUniversalTime())
                    .WithSimpleSchedule(schedule => schedule.WithRepeatCount(0)
                        .WithMisfireHandlingInstructionFireNow());
            }
            else
            {
                if (request.CronExpression!.Length > 120 ||
                    !CronExpression.IsValidExpression(request.CronExpression))
                    throw new BadRequestException("cronExpression không hợp lệ. Quartz dùng 6 hoặc 7 trường, bắt đầu bằng giây.");

                var timeZone = ResolveTimeZone(request.TimeZoneId);
                var cron = new CronExpression(request.CronExpression) { TimeZone = timeZone };
                if (cron.GetNextValidTimeAfter(now) is null)
                    throw new BadRequestException("cronExpression không có lần chạy nào trong tương lai.");

                // Skip missed recurring runs instead of sending a burst after downtime.
                triggerBuilder.StartAt(now).WithCronSchedule(request.CronExpression, schedule => schedule
                    .InTimeZone(timeZone)
                    .WithMisfireHandlingInstructionDoNothing());
            }

            // A JSON string also works with AdoJobStore's UseProperties=true.
            var job = JobBuilder.Create<FirebaseNotificationJob>()
                .WithIdentity(jobKey)
                .UsingJobData(FirebaseNotificationJob.PayloadKey, JsonSerializer.Serialize(request))
                .UsingJobData($"user_id", Id.ToString())
                .Build();
            var trigger = triggerBuilder.Build();
            var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            var firstRun = await scheduler.ScheduleJob(job, trigger, cancellationToken);

            logger.LogInformation(
                "Scheduled notification {NotificationId} for {FirstRunUtc}",
                id,
                firstRun);
            return ToResponse(id, request, firstRun, TriggerState.Normal.ToString());
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to schedule notification");
            throw;
        }
    }

    public async Task<ScheduledNotificationResponse?> GetAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var job = await scheduler.GetJobDetail(JobKeyFor(id), cancellationToken);
        if (job is null)
            return null;

        var request = JsonSerializer.Deserialize<ScheduleNotificationRequest>(
            job.JobDataMap.GetString(FirebaseNotificationJob.PayloadKey)!)!;
        var trigger = await scheduler.GetTrigger(TriggerKeyFor(id), cancellationToken);
        var state = await scheduler.GetTriggerState(TriggerKeyFor(id), cancellationToken);

        return ToResponse(id, request, trigger?.GetNextFireTimeUtc(), state.ToString());
    }

    public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        return await scheduler.DeleteJob(JobKeyFor(id), cancellationToken);
    }

    private static JobKey JobKeyFor(Guid id) => new(id.ToString("N"), Group);
    private static TriggerKey TriggerKeyFor(Guid id) => new(id.ToString("N"), Group);

    private static ScheduledNotificationResponse ToResponse(
        Guid id, ScheduleNotificationRequest request, DateTimeOffset? nextRun, string state) =>
        new(id, request.Title, request.Body, request.RunAt?.ToUniversalTime(),
            request.RunAt.HasValue ? null : request.CronExpression,
            request.RunAt.HasValue ? null : request.TimeZoneId,
            nextRun?.ToUniversalTime(), state);

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId) || timeZoneId.Length > 80)
            throw new BadRequestException("timeZoneId không hợp lệ.");

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            throw new BadRequestException("timeZoneId không hợp lệ. Ví dụ: Asia/Ho_Chi_Minh hoặc UTC.");
        }
    }

    private static void ValidatePayload(ScheduleNotificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
            throw new BadRequestException("title và body không được để trống.");

        var hasToken = !string.IsNullOrWhiteSpace(request.Token);
        var hasTopic = !string.IsNullOrWhiteSpace(request.Topic);
        if (hasToken == hasTopic)
            throw new BadRequestException("Cung cấp đúng một nơi nhận: token hoặc topic.");

        if (hasToken && (request.Token!.Length > 4096 || request.Token.Any(char.IsWhiteSpace)))
            throw new BadRequestException("token thiết bị không hợp lệ.");
        if (hasTopic && (request.Topic!.Length > 900 ||
            !Regex.IsMatch(request.Topic, @"\A[a-zA-Z0-9\-_.~%]+\z", RegexOptions.CultureInvariant)))
            throw new BadRequestException("topic không hợp lệ; chỉ gửi tên topic, không kèm /topics/.");

        if (request.Data?.Any(pair => string.IsNullOrWhiteSpace(pair.Key) || pair.Value is null ||
            pair.Key is "from" or "message_type" ||
            pair.Key.StartsWith("google.", StringComparison.Ordinal) ||
            pair.Key.StartsWith("gcm.", StringComparison.Ordinal)) == true)
            throw new BadRequestException("data chứa khóa dành riêng cho FCM hoặc giá trị không hợp lệ.");

        // Conservative limit on the serialized notification/data payload (UTF-8 bytes).
        var payload = JsonSerializer.SerializeToUtf8Bytes(new
        {
            notification = new { title = request.Title, body = request.Body },
            data = request.Data
        });
        if (payload.Length > (hasTopic ? 2048 : 4096))
            throw new BadRequestException("Nội dung thông báo vượt giới hạn FCM (topic: 2048 byte; token: 4096 byte).");
    }
}
