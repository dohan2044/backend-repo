using System.Reflection.Emit;
using Journey_of_faith.Domain.entities;

namespace Domain.Entities;

// 1. Danh mục kinh nguyện
public class FaithPrayerCategory : AuditableEntity
{
    private readonly List<FaithPrayer> _prayers = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int SortOrder { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;

    public virtual IReadOnlyCollection<FaithPrayer> Prayers => _prayers.AsReadOnly();

    public void AddPrayer(FaithPrayer prayer) => _prayers.Add(prayer);
}

// 2. Nội dung kinh nguyện
public class FaithPrayer : AuditableEntity
{
    public int CategoryId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Subtitle { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string? Source { get; private set; }
    public int SortOrder { get; private set; } = 0;
    public bool IsPublished { get; private set; } = false;

    public virtual FaithPrayerCategory Category { get; private set; } = null!;
}

// 3. Nhóm sinh viên
public class StudentGroup : AuditableEntity
{
    private readonly List<StudentGroupMember> _members = new();

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? ContactEmail { get; private set; }
    public bool IsActive { get; private set; } = true;

    public virtual IReadOnlyCollection<StudentGroupMember>   Members => _members.AsReadOnly();

    public void AddMember(StudentGroupMember member) => _members.Add(member);

    private StudentGroup() { } // Required by EF Core
    public StudentGroup(string name, string code, string? description = null, string? logoUrl = null, string? contactEmail = null)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên nhóm không được để trống.", nameof(name));
        Name = name;
        Code = code;
        Description = description;
        LogoUrl = logoUrl;
        ContactEmail = contactEmail;
    }
}

// 4. Thành viên nhóm sinh viên
public class StudentGroupMember : AuditableEntity
{
    public int GroupId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; } = "Member";
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;
    public byte Status { get; private set; } = 0;

    public virtual StudentGroup Group { get; private set; } = null!;
}

// 5. Ý nguyện cầu nguyện
public class PrayerIntention : AuditableEntity
{
    private readonly List<PrayerIntentionPrayer> _prayingUsers = new();

    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public bool IsAnonymous { get; private set; } = false;
    public byte Status { get; private set; } = 0;
    public int PrayerCount { get; private set; } = 0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public virtual IReadOnlyCollection<PrayerIntentionPrayer> PrayingUsers => _prayingUsers.AsReadOnly();

    public void AddPrayingUser(PrayerIntentionPrayer prayer) => _prayingUsers.Add(prayer);
}

// 6. Người tham gia hiệp ý cầu nguyện
public class PrayerIntentionPrayer : AuditableEntity
{
    public int IntentionId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public virtual PrayerIntention Intention { get; private set; } = null!;
}

// 7. Hoạt động bác ái
public class CharityActivity : AuditableEntity
{
    private readonly List<CharityRegistration> _registrations = new();

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public int? MaxParticipants { get; private set; }
    public byte Status { get; private set; } = 0;
    public string? CoverImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public virtual IReadOnlyCollection<CharityRegistration> Registrations => _registrations.AsReadOnly();

    public void AddRegistration(CharityRegistration registration) => _registrations.Add(registration);
}

// 8. Đăng ký hoạt động bác ái
public class CharityRegistration : AuditableEntity
{
    public int ActivityId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;
    public byte Status { get; private set; } = 0;
    public string? Note { get; private set; }

    public virtual CharityActivity Activity { get; private set; } = null!;
}

// 9. Sự kiện sinh viên
public class StudentEvent : AuditableEntity
{
    private readonly List<StudentEventRegistration> _registrations = new();
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public int? MaxParticipants { get; private set; }
    public DateTime? RegistrationDeadline { get; private set; }
    public byte Status { get; private set; } = 0;
    public string? CoverImageUrl { get; private set; }

    public virtual IReadOnlyCollection<StudentEventRegistration> Registrations => _registrations.AsReadOnly();

    public void AddRegistration(StudentEventRegistration registration) => _registrations.Add(registration);

}

// 10. Đăng ký tham gia sự kiện sinh viên
public class StudentEventRegistration : AuditableEntity
{
    public int EventId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;
    public byte Status { get; private set; } = 0;

    public virtual StudentEvent Event { get; private set; } = null!;
}