namespace Journey_of_faith.Application.common.dtos.church;

public sealed class DioceseViewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? Thumbnail { get; set; }
    public int ChurchCount { get; set; }
}

public sealed class ChurchListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int DioceseId { get; set; }
    public string? DioceseName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsFollowed { get; set; }
    public IReadOnlyList<MassScheduleViewDto> MassSchedules { get; set; } = [];
}

public sealed class MassScheduleTodayDto
{
    public string Time { get; set; } = string.Empty;
    public string MassName { get; set; } = string.Empty;
    public string ChurchName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class ReminderSettingDto
{
    public bool MassReminderEnabled { get; set; }
    public int MinutesBefore { get; set; }
    public string? SpeechGender { get; set; }
    public double? SpeechSpeed { get; set; }
}

public sealed class DailyWordViewDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Title { get; set; }
    public string BibleContent { get; set; } = string.Empty;
    public string? Gospel { get; set; }
    public bool? IsShortWord { get; set; }
}
