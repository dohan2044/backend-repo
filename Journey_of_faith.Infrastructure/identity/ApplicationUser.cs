using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.entities.events;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.entities.notifications;
using Journey_of_faith.Domain.entities.prayer;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.entities.social;
using Microsoft.AspNetCore.Identity;

namespace Journey_of_faith.Infrastructure.identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Avatar { get; set; }

        public int? ChurchId { get; set; }
        public int? ProvinceId { get; set; }
        public int? SchoolId { get; set; }
        public int Score { get; set; } = 0;
        public int DayStreak { get; set; } = 0;
        // Audit fields
        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierUserId { get; set; }
        public DateTime LastModificationTime { get; set; }
        public Guid? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation
        public Church? Church { get; set; }
        public Province? Province { get; set; }
        public School? School { get; set; }

    }
}
