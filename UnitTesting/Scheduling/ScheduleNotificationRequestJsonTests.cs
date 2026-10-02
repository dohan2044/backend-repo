using System.Text.Json;
using Journey_of_faith.Application.usecases.notifications;

namespace UnitTesting.Scheduling;

public sealed class ScheduleNotificationRequestJsonTests
{
    private static readonly JsonSerializerOptions WebOptions = new(JsonSerializerDefaults.Web);

    [Theory]
    [InlineData("2030-01-02T08:30:00+07:00", 7)]
    [InlineData("2030-01-02T08:30:00Z", 0)]
    [InlineData("2030-01-02T08:30:00-05:00", -5)]
    public void RunAt_AcceptsExplicitOffsetAndPreservesIt(string timestamp, int offsetHours)
    {
        var json = JsonSerializer.Serialize(new { runAt = timestamp });

        var request = JsonSerializer.Deserialize<ScheduleNotificationRequest>(json, WebOptions);

        Assert.NotNull(request);
        Assert.Equal(new DateTimeOffset(2030, 1, 2, 8, 30, 0, TimeSpan.FromHours(offsetHours)), request.RunAt);
        Assert.Equal(TimeSpan.FromHours(offsetHours), request.RunAt!.Value.Offset);
    }

    [Theory]
    [InlineData("2030-01-02T08:30:00")]
    [InlineData("2030-01-02")]
    [InlineData("2030-13-02T08:30:00+07:00")]
    [InlineData("2030-01-02T08:30:00+25:00")]
    [InlineData("2030-01-02T08:30:00+0700")]
    [InlineData("not-a-date")]
    [InlineData("")]
    public void RunAt_RejectsAmbiguousOrInvalidTimestamp(string timestamp)
    {
        var json = JsonSerializer.Serialize(new { runAt = timestamp });

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ScheduleNotificationRequest>(json, WebOptions));
    }

    [Theory]
    [InlineData("{\"runAt\":123}")]
    [InlineData("{\"runAt\":true}")]
    [InlineData("{\"runAt\":{}}")]
    public void RunAt_RejectsNonStringJsonValues(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ScheduleNotificationRequest>(json, WebOptions));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"runAt\":null}")]
    public void RunAt_AllowsMissingOrNullForCronRequests(string json)
    {
        var request = JsonSerializer.Deserialize<ScheduleNotificationRequest>(json, WebOptions);

        Assert.NotNull(request);
        Assert.Null(request.RunAt);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RunAt_RoundTripsOffsetAndSubsecondPrecisionForStorageAndApi(bool useWebDefaults)
    {
        var options = useWebDefaults ? WebOptions : new JsonSerializerOptions();
        var runAt = new DateTimeOffset(2030, 1, 2, 8, 30, 0, TimeSpan.FromHours(7)).AddTicks(1234567);
        var request = new ScheduleNotificationRequest
        {
            Title = "Prayer reminder",
            Body = "It is time to pray.",
            Topic = "faith-reminders",
            RunAt = runAt
        };

        var json = JsonSerializer.Serialize(request, options);
        var restored = JsonSerializer.Deserialize<ScheduleNotificationRequest>(json, options);

        Assert.NotNull(restored);
        Assert.Equal(runAt, restored.RunAt);
        Assert.Equal(runAt.Offset, restored.RunAt!.Value.Offset);
        Assert.Equal(request.Topic, restored.Topic);
    }
}
