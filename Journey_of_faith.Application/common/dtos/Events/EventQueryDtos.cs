using System.Text.Json.Serialization;

namespace Journey_of_faith.Application.common.dtos.Events;

public sealed class EventFilterDto
{
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class EventViewDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFollowed { get; set; }
    public int FollowerCount { get; set; }
}

public sealed class EventDetailsDto : EventViewDto
{
    public int ParticipantCount { get; set; }
    public List<EventCategoryDto> Categories { get; set; } = [];
    public List<EventImageDto> Images { get; set; } = [];
}

public sealed class EventCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public string? Events { get; set; }
    public List<EventViewDto> EventsList { get; set; } = [];
}

public sealed class EventImageDto
{
    public long Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public sealed class EventCommentDto
{
    public string? Username { get; set; }
    public string Comment { get; set; } = string.Empty;
}
