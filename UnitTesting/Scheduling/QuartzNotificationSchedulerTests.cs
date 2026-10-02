using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.notifications;
using Journey_of_faith.Infrastructure.scheduling;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace UnitTesting.Scheduling;

public sealed class QuartzNotificationSchedulerTests : IAsyncLifetime
{
    private static readonly DateTimeOffset Now = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private IScheduler _scheduler = null!;
    private QuartzNotificationScheduler _service = null!;

    public async Task InitializeAsync()
    {
        // Never start the scheduler: these tests inspect triggers without executing sends.
        var factory = new StdSchedulerFactory(new NameValueCollection
        {
            ["quartz.scheduler.instanceName"] = $"notification-tests-{Guid.NewGuid():N}",
            ["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz",
            ["quartz.threadPool.maxConcurrency"] = "1"
        });
        _scheduler = await factory.GetScheduler();
        _service = new QuartzNotificationScheduler(factory, new FixedTimeProvider(Now));
    }

    public Task DisposeAsync() => _scheduler.Shutdown();

    [Fact]
    public async Task OneOff_NormalizesOffsetAndCreatesSingleFireTrigger()
    {
        var request = NewOneOff();
        request.RunAt = new DateTimeOffset(2030, 1, 2, 8, 30, 0, TimeSpan.FromHours(7));
        var expectedUtc = new DateTimeOffset(2030, 1, 2, 1, 30, 0, TimeSpan.Zero);

        var created = await _service.ScheduleAsync(request);
        var retrieved = await _service.GetAsync(created.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(request.Title, retrieved.Title);
        Assert.Equal(expectedUtc, retrieved.RunAtUtc);
        Assert.Equal(TimeSpan.Zero, retrieved.RunAtUtc!.Value.Offset);
        Assert.Equal(expectedUtc, retrieved.NextRunAtUtc);
        Assert.Null(retrieved.CronExpression);

        var keys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
        var triggers = await _scheduler.GetTriggersOfJob(Assert.Single(keys));
        var trigger = Assert.IsAssignableFrom<ISimpleTrigger>(Assert.Single(triggers));
        Assert.Equal(0, trigger.RepeatCount);
        Assert.Null(trigger.GetFireTimeAfter(expectedUtc));
    }

    [Fact]
    public async Task Cron_UsesRequestedTimezoneForNextOccurrence()
    {
        var request = NewOneOff();
        request.RunAt = null;
        request.CronExpression = "0 30 20 * * ?";
        request.TimeZoneId = "Asia/Ho_Chi_Minh";

        var created = await _service.ScheduleAsync(request);

        Assert.Equal(new DateTimeOffset(2030, 1, 1, 13, 30, 0, TimeSpan.Zero), created.NextRunAtUtc);
        Assert.Null(created.RunAtUtc);
        Assert.Equal(request.CronExpression, created.CronExpression);

        var keys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
        var triggers = await _scheduler.GetTriggersOfJob(Assert.Single(keys));
        var trigger = Assert.IsAssignableFrom<ICronTrigger>(Assert.Single(triggers));
        Assert.Equal(TimeSpan.FromHours(7), trigger.TimeZone.GetUtcOffset(Now));
    }

    [Fact]
    public async Task Payload_IsStoredAsJsonAndDoesNotKeepMutableRequestReferences()
    {
        var request = NewOneOff();
        request.Data = new Dictionary<string, string> { ["articleId"] = "123" };
        await _service.ScheduleAsync(request);
        request.Title = "Changed after scheduling";
        request.Data["articleId"] = "456";

        var keys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
        var job = await _scheduler.GetJobDetail(Assert.Single(keys));
        Assert.NotNull(job);
        var json = Assert.IsType<string>(job.JobDataMap[FirebaseNotificationJob.PayloadKey]);
        var payload = JsonSerializer.Deserialize<ScheduleNotificationRequest>(json);

        Assert.NotNull(payload);
        Assert.Equal("Prayer reminder", payload.Title);
        Assert.Equal("123", payload.Data!["articleId"]);
        Assert.Equal("faith-reminders", payload.Topic);
    }

    [Fact]
    public async Task Cancel_RemovesJobAndFutureTriggers()
    {
        var created = await _service.ScheduleAsync(NewOneOff());

        Assert.True(await _service.CancelAsync(created.Id));
        Assert.Null(await _service.GetAsync(created.Id));
        Assert.False(await _service.CancelAsync(created.Id));
        Assert.Empty(await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()));
        Assert.Empty(await _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()));
    }

    [Fact]
    public async Task Get_UnknownIdReturnsNull()
    {
        Assert.Null(await _service.GetAsync(Guid.NewGuid()));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", " ")]
    [InlineData("device-token", "faith-reminders")]
    public async Task RejectsMissingOrConflictingTargets(string? token, string? topic)
    {
        var request = NewOneOff();
        request.Token = token;
        request.Topic = topic;

        await AssertRejectedWithoutScheduling(request);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RejectsMissingOrConflictingScheduleModes(bool supplyBoth)
    {
        var request = NewOneOff();
        request.RunAt = supplyBoth ? Now.AddHours(1) : null;
        request.CronExpression = supplyBoth ? "0 0 20 * * ?" : null;

        await AssertRejectedWithoutScheduling(request);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task RejectsPastOrCurrentOneOffTime(int secondsFromNow)
    {
        var request = NewOneOff();
        request.RunAt = Now.AddSeconds(secondsFromNow);

        await AssertRejectedWithoutScheduling(request);
    }

    [Theory]
    [InlineData("not a cron", "Asia/Ho_Chi_Minh")]
    [InlineData("0 0 20 * * ?", "Invalid/Unknown_Timezone")]
    [InlineData("0 0 0 1 1 ? 2020", "Asia/Ho_Chi_Minh")]
    public async Task RejectsInvalidCronTimezoneOrScheduleWithoutFutureOccurrence(string cron, string timezone)
    {
        var request = NewOneOff();
        request.RunAt = null;
        request.CronExpression = cron;
        request.TimeZoneId = timezone;

        await AssertRejectedWithoutScheduling(request);
    }

    [Theory]
    [InlineData(null, "Body")]
    [InlineData("", "Body")]
    [InlineData(" \t", "Body")]
    [InlineData("Title", null)]
    [InlineData("Title", "")]
    [InlineData("Title", " \t")]
    public async Task RejectsNullEmptyOrWhitespaceNotificationText(string? title, string? body)
    {
        var request = NewOneOff();
        request.Title = title!;
        request.Body = body!;

        await AssertRejectedWithoutScheduling(request);
    }

    [Theory]
    [InlineData("/topics/faith-reminders")]
    [InlineData("faith reminders")]
    [InlineData("faith/reminders")]
    [InlineData("faith-reminders\n")]
    public async Task RejectsMalformedTopic(string topic)
    {
        var request = NewOneOff();
        request.Topic = topic;

        await AssertRejectedWithoutScheduling(request);
    }

    [Theory]
    [InlineData("from", "server")]
    [InlineData("message_type", "notification")]
    [InlineData("google.custom", "value")]
    [InlineData("gcm.custom", "value")]
    [InlineData(" ", "value")]
    [InlineData("articleId", null)]
    public async Task RejectsReservedDataKeysOrInvalidDataValues(string key, string? value)
    {
        var request = NewOneOff();
        request.Data = new Dictionary<string, string> { [key] = value! };

        await AssertRejectedWithoutScheduling(request);
    }

    [Theory]
    [InlineData(false, 2049, 'a')]
    [InlineData(true, 4097, 'a')]
    [InlineData(true, 1400, '\u1ed9')]
    public async Task RejectsPayloadOverTargetByteLimit(bool useToken, int bodyLength, char character)
    {
        var request = NewOneOff();
        request.Token = useToken ? "test-device-token" : null;
        request.Topic = useToken ? null : "faith-reminders";
        request.Body = new string(character, bodyLength);

        await AssertRejectedWithoutScheduling(request);
    }

    private async Task AssertRejectedWithoutScheduling(ScheduleNotificationRequest request)
    {
        var failure = await Assert.ThrowsAsync<BadRequestException>(() => _service.ScheduleAsync(request));
        Assert.Equal(HttpStatusCode.BadRequest, failure.StatusCode);
        Assert.Empty(await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()));
    }

    private static ScheduleNotificationRequest NewOneOff() => new()
    {
        Title = "Prayer reminder",
        Body = "It is time to pray.",
        Topic = "faith-reminders",
        RunAt = Now.AddHours(1)
    };

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
