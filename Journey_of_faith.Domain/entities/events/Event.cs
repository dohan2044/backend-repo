using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace Journey_of_faith.Domain.entities.events
{
    public class Event : AuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ImageUrl { get; set; }

        private readonly List<EventCategoryMapping> _categoryMappings = new();
        private readonly List<EventImage> _eventImages = new();
        private readonly List<EventComment> _eventComments = new();
        private readonly List<EventParticipant> _eventParticipants = new();
        private readonly List<EventFollower> _eventFollowers = new();
        private readonly List<UserEvent> _userEvents = new();
        private readonly List<EventNotification> _notifications = new();

        public IReadOnlyCollection<EventCategoryMapping> CategoryMappings => _categoryMappings.AsReadOnly();
        public IReadOnlyCollection<EventImage> Images => _eventImages.AsReadOnly();
        public IReadOnlyCollection<EventComment> Comments => _eventComments.AsReadOnly();
        public IReadOnlyCollection<EventParticipant> Participants => _eventParticipants.AsReadOnly();
        public IReadOnlyCollection<EventFollower> Followers => _eventFollowers.AsReadOnly();
        public IReadOnlyCollection<UserEvent> UserEvents => _userEvents.AsReadOnly();
        public IReadOnlyCollection<EventNotification> Notifications => _notifications.AsReadOnly();
    }
}
