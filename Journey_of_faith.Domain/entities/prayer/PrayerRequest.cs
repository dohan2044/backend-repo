using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.prayer
{
    public class PrayerRequest : AuditableEntity
    {
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? RequestContent { get; set; }
        public bool? IsAnonymous { get; set; }

        private readonly List<PrayerComment> _comments = new();

        public IReadOnlyCollection<PrayerComment> Comments => _comments.AsReadOnly();

        public void AddComment(PrayerComment comment) => _comments.Add(comment);
    }
}
