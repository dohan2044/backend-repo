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

    public void AddMember(StudentGroupMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (Id > 0 && member.GroupId != Id)
        {
            throw new ArgumentException("Thành viên không thuộc nhóm này.", nameof(member));
        }

        if (_members.Any(existing => existing.UserId == member.UserId))
        {
            throw new InvalidOperationException("Người dùng đã là thành viên của nhóm.");
        }

        _members.Add(member);
    }

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
    private StudentGroupMember() {}
    public StudentGroupMember(int groupId, Guid userId, string role = "Member")
    {
        if(groupId <= 0) 
            throw new ArgumentException("GroupId phải lớn hơn 0", nameof(groupId));
        if(userId == Guid.Empty)
            throw new ArgumentException("UserId không được để trống", nameof(userId));
        if(string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Vai trò không được để trống", nameof(role));
        GroupId = groupId;
        UserId = userId;
        Role = role.Trim();
        JoinedAt = DateTime.UtcNow;
    }

    public void ChangeRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Vai trò không được để trống", nameof(role));

        Role = role.Trim();
    }
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

    private CharityActivity() { }

    public CharityActivity(
        string title,
        string description,
        string location,
        DateTime startAt,
        DateTime endAt,
        int? maxParticipants = null,
        string? coverImageUrl = null)
    {
        SetDetails(title, description, location, startAt, endAt, maxParticipants, coverImageUrl);
    }

    public void UpdateDetails(
        string title,
        string description,
        string location,
        DateTime startAt,
        DateTime endAt,
        int? maxParticipants,
        string? coverImageUrl)
    {
        SetDetails(title, description, location, startAt, endAt, maxParticipants, coverImageUrl);
        LastModificationTime = DateTime.UtcNow;
    }

    public void AddRegistration(CharityRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (Id > 0 && registration.ActivityId != Id)
            throw new ArgumentException("Đăng ký không thuộc hoạt động bác ái này.", nameof(registration));
        if (_registrations.Any(existing => existing.UserId == registration.UserId))
            throw new InvalidOperationException("Người dùng đã đăng ký hoạt động này.");
        if (MaxParticipants.HasValue &&
            _registrations.Count(existing => existing.IsDeleted != true) >= MaxParticipants.Value)
            throw new InvalidOperationException("Hoạt động đã đủ số lượng người tham gia.");

        _registrations.Add(registration);
    }

    private void SetDetails(
        string title,
        string description,
        string location,
        DateTime startAt,
        DateTime endAt,
        int? maxParticipants,
        string? coverImageUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Tiêu đề không được để trống.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Mô tả không được để trống.", nameof(description));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Địa điểm không được để trống.", nameof(location));
        if (endAt <= startAt)
            throw new ArgumentException("Thời gian kết thúc phải sau thời gian bắt đầu.", nameof(endAt));
        if (maxParticipants <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxParticipants), "Số người tham gia phải lớn hơn 0.");

        Title = title.Trim();
        Description = description.Trim();
        Location = location.Trim();
        StartAt = startAt;
        EndAt = endAt;
        MaxParticipants = maxParticipants;
        CoverImageUrl = coverImageUrl;
    }
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

    private CharityRegistration() { }

    public CharityRegistration(int activityId, Guid userId, string? note = null)
    {
        if (activityId <= 0)
            throw new ArgumentOutOfRangeException(nameof(activityId), "ActivityId phải lớn hơn 0.");
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không được để trống.", nameof(userId));

        ActivityId = activityId;
        UserId = userId;
        Note = note;
        RegisteredAt = DateTime.UtcNow;
    }
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

    private StudentEvent() { }

    public StudentEvent(
        string title,
        string description,
        string location,
        DateTime startAt,
        DateTime endAt,
        int? maxParticipants = null,
        DateTime? registrationDeadline = null,
        string? coverImageUrl = null)
    {
        SetDetails(title, description, location, startAt, endAt, maxParticipants, registrationDeadline, coverImageUrl);
    }

    public void UpdateDetails(
        string title,
        string description,
        string location,
        DateTime startAt,
        DateTime endAt,
        int? maxParticipants,
        DateTime? registrationDeadline,
        string? coverImageUrl)
    {
        SetDetails(title, description, location, startAt, endAt, maxParticipants, registrationDeadline, coverImageUrl);
        LastModificationTime = DateTime.UtcNow;
    }

    public void AddRegistration(StudentEventRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (Id > 0 && registration.EventId != Id)
            throw new ArgumentException("Đăng ký không thuộc sự kiện này.", nameof(registration));
        if (_registrations.Any(existing => existing.UserId == registration.UserId))
            throw new InvalidOperationException("Người dùng đã đăng ký sự kiện này.");
        if (MaxParticipants.HasValue &&
            _registrations.Count(existing => existing.IsDeleted != true) >= MaxParticipants.Value)
            throw new InvalidOperationException("Sự kiện đã đủ số lượng người tham gia.");
        if (RegistrationDeadline.HasValue && RegistrationDeadline.Value < DateTime.UtcNow)
            throw new InvalidOperationException("Đã quá hạn đăng ký sự kiện.");

        _registrations.Add(registration);
    }

    private void SetDetails(
        string title,
        string description,
        string location,
        DateTime startAt,
        DateTime endAt,
        int? maxParticipants,
        DateTime? registrationDeadline,
        string? coverImageUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Tiêu đề không được để trống.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Mô tả không được để trống.", nameof(description));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Địa điểm không được để trống.", nameof(location));
        if (endAt <= startAt)
            throw new ArgumentException("Thời gian kết thúc phải sau thời gian bắt đầu.", nameof(endAt));
        if (maxParticipants <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxParticipants), "Số người tham gia phải lớn hơn 0.");
        if (registrationDeadline.HasValue && registrationDeadline.Value > startAt)
            throw new ArgumentException("Hạn đăng ký không thể sau thời gian bắt đầu.", nameof(registrationDeadline));
        if (maxParticipants.HasValue &&
            _registrations.Count(registration => registration.IsDeleted != true) > maxParticipants.Value)
            throw new InvalidOperationException("Số người tham gia tối đa thấp hơn số đăng ký hiện tại.");

        Title = title.Trim();
        Description = description.Trim();
        Location = location.Trim();
        StartAt = startAt;
        EndAt = endAt;
        MaxParticipants = maxParticipants;
        RegistrationDeadline = registrationDeadline;
        CoverImageUrl = coverImageUrl;
    }

}

// 10. Đăng ký tham gia sự kiện sinh viên
public class StudentEventRegistration : AuditableEntity
{
    public int EventId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;
    public byte Status { get; private set; } = 0;

    public virtual StudentEvent Event { get; private set; } = null!;

    private StudentEventRegistration() { }

    public StudentEventRegistration(int eventId, Guid userId)
    {
        if (eventId <= 0)
            throw new ArgumentOutOfRangeException(nameof(eventId), "EventId phải lớn hơn 0.");
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không được để trống.", nameof(userId));

        EventId = eventId;
        UserId = userId;
        RegisteredAt = DateTime.UtcNow;
    }
}