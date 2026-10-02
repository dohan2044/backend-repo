using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Journey_of_faith.Application.usecases.notifications;

public sealed class ScheduleNotificationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? Topic { get; set; }
    public Dictionary<string, string>? Data { get; set; }

    // Supply exactly one of RunAt (with UTC offset) or CronExpression.
    [JsonConverter(typeof(ExplicitOffsetDateTimeConverter))]
    public DateTimeOffset? RunAt { get; set; }
    public string? CronExpression { get; set; }
    public string TimeZoneId { get; set; } = "Asia/Ho_Chi_Minh";
}

public sealed record ScheduledNotificationResponse(
    Guid Id,
    string Title,
    string Body,
    DateTimeOffset? RunAtUtc,
    string? CronExpression,
    string? TimeZoneId,
    DateTimeOffset? NextRunAtUtc,
    string State);

// Reject ambiguous local timestamps so changing server timezone cannot change a schedule.
public sealed class ExplicitOffsetDateTimeConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String ||
            !Regex.IsMatch(reader.GetString()!, @"(?:Z|[+-]\d{2}:\d{2})\z", RegexOptions.CultureInvariant) ||
            !reader.TryGetDateTimeOffset(out var value))
            throw new JsonException("runAt phải là ISO 8601 kèm UTC offset, ví dụ 2030-01-01T20:30:00+07:00.");

        return value;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
}
