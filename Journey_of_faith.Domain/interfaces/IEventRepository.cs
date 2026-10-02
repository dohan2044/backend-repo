using Journey_of_faith.Domain.entities.events;

namespace Journey_of_faith.Domain.interfaces
{
    public class CreateEventPayload
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status {get; set;} = string.Empty;
        public string? ImageUrl { get; set; }
        public Guid CreatorUserId { get; set; }
        public List<int> CategoryIds { get; set; } = [];
        public List<string> ImageUrls { get; set; } = [];
    }

    public class UpdateEventPayload
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ImageUrl { get; set; }
        public Guid LastModifierUserId { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<string>? ImageUrls { get; set; }
    }

    public interface IEventRepository
    {
        Task<int> CreateCategoryAsync(string categoryName);

        Task<int> CreateEventAsync( string payload);
        Task<bool> UpdateEventAsync(UpdateEventPayload payload);
        Task<bool> DeleteEventAsync(int eventId);

        Task<bool> FollowEventAsync(Guid userId, int eventId);
        Task<bool> UnfollowEventAsync(Guid userId, int eventId);

        Task<bool> CreateEventCommentAsync(EventComment comment);
    }
}
