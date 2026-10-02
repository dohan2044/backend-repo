
using Journey_of_faith.Domain.entities.events;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.entities.notifications;
using Journey_of_faith.Domain.entities.prayer;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.entities.social;

namespace Journey_of_faith.Domain.entities
{
    public partial class User
    {
        public Guid Id { get; set; }
        public string Name { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string? Avatar { get; private set; }
        public string Role { get; set; }
        public string PasswordHash { get; private set; } = string.Empty;
        public int? RoleId { get; private set; }
        public List<string>? RoleName { get; private set; }
        public int? ChurchId { get; private set; }

        public int? ProvinceId { get; private set; }
        public string? ProvinceName { get; private set; }
        public int? SchoolId { get; private set; }
        public Guid CreatorUserId { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int AccessFailedCount { get; set; } = 0;
        public bool? IsDeleted { get; set; }
        public int Score { get; set; } = 0;
        public int DayStreak { get; set; } = 0;
        public bool EmailConfirmed { get; set; } = false;
        public bool LockoutEnabled { get; set; } = true;
        public bool TwoFactorEnabled { get; set; } = false;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public string? PhoneNumber { get; set; } // Nếu có cột PhoneNumber thì thêm

        private readonly List<UserChurch> _userChurches = new();
        private readonly List<Friendship> _friendships = new();
        private readonly List<Friendship> _friendOf = new();
        private readonly List<GroupMember> _groupMembers = new();
        private readonly List<Playlist> _playlists = new();
        private readonly List<UserFavoriteSong> _favoriteSongs = new();
        private readonly List<ListeningHistory> _listeningHistories = new();
        private readonly List<EventComment> _eventComments = new();
        private readonly List<UserEvent> _userEvents = new();
        private readonly List<QuizAttempt> _quizAttempts = new();
        private readonly List<PrayerRequest> _prayerRequests = new();
        private readonly List<PrayerComment> _prayerComments = new();
        private readonly List<DeviceToken> _deviceTokens = new();
        private readonly List<NotificationPreference> _notificationPreferences = new();
        private readonly List<ReminderSetting> _reminderSettings = new();
        private readonly List<Conversation> _createdConversations = new();
        private readonly List<ConversationParticipant> _conversationParticipants = new();
        private readonly List<Message> _sentMessages = new();
        private readonly List<MessageReaction> _messageReactions = new();
        private readonly List<UserActive> _userActives = new();

        public IReadOnlyCollection<UserChurch> UserChurches => _userChurches.AsReadOnly();
        public IReadOnlyCollection<Friendship> Friendships => _friendships.AsReadOnly();
        public IReadOnlyCollection<Friendship> FriendOf => _friendOf.AsReadOnly();
        public IReadOnlyCollection<GroupMember> GroupMembers => _groupMembers.AsReadOnly();
        public IReadOnlyCollection<Playlist> Playlists => _playlists.AsReadOnly();
        public IReadOnlyCollection<UserFavoriteSong> FavoriteSongs => _favoriteSongs.AsReadOnly();
        public IReadOnlyCollection<ListeningHistory> ListeningHistories => _listeningHistories.AsReadOnly();
        public IReadOnlyCollection<EventComment> EventComments => _eventComments.AsReadOnly();
        public IReadOnlyCollection<UserEvent> UserEvents => _userEvents.AsReadOnly();
        public IReadOnlyCollection<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();
        public IReadOnlyCollection<PrayerRequest> PrayerRequests => _prayerRequests.AsReadOnly();
        public IReadOnlyCollection<PrayerComment> PrayerComments => _prayerComments.AsReadOnly();
        public IReadOnlyCollection<DeviceToken> DeviceTokens => _deviceTokens.AsReadOnly();
        public IReadOnlyCollection<NotificationPreference> NotificationPreferences => _notificationPreferences.AsReadOnly();
        public IReadOnlyCollection<ReminderSetting> ReminderSettings => _reminderSettings.AsReadOnly();
        public IReadOnlyCollection<Conversation> CreatedConversations => _createdConversations.AsReadOnly();
        public IReadOnlyCollection<ConversationParticipant> ConversationParticipants => _conversationParticipants.AsReadOnly();
        public IReadOnlyCollection<Message> SentMessages => _sentMessages.AsReadOnly();
        public IReadOnlyCollection<MessageReaction> MessageReactions => _messageReactions.AsReadOnly();
        public IReadOnlyCollection<UserActive> userActives => _userActives.AsReadOnly();

        public User()
        {

        }
        public User(
            string email, string username, string avatar, List<string> roleName, string provinceName, int score, int dayStreak
        )
        {
            Email = email;
            Username = username;
            Avatar = avatar;
            RoleName = roleName;
            ProvinceName = provinceName;
            Score = score;
            DayStreak = dayStreak;
        }
        public User(string name, string email, string username, string password, string? avatar)
        {
            Name = name;
            Email = email;
            Username = username;
            Password = password;
            Avatar = avatar;
        }

        public User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }

        public static User Create(string username, string password, string email)
            => new User(username, password, email);

        public void AddUserChurch(UserChurch value) => _userChurches.Add(value);
        public void AddFriendship(Friendship value) => _friendships.Add(value);
        public void AddFriendOf(Friendship value) => _friendOf.Add(value);
        public void AddGroupMember(GroupMember value) => _groupMembers.Add(value);
        public void AddPlaylist(Playlist value) => _playlists.Add(value);
        public void AddFavoriteSong(UserFavoriteSong value) => _favoriteSongs.Add(value);
        public void AddListeningHistory(ListeningHistory value) => _listeningHistories.Add(value);
        public void AddEventComment(EventComment value) => _eventComments.Add(value);
        public void AddUserEvent(UserEvent value) => _userEvents.Add(value);
        public void AddQuizAttempt(QuizAttempt value) => _quizAttempts.Add(value);
        public void AddPrayerRequest(PrayerRequest value) => _prayerRequests.Add(value);
        public void AddPrayerComment(PrayerComment value) => _prayerComments.Add(value);
        public void AddDeviceToken(DeviceToken value) => _deviceTokens.Add(value);
        public void AddNotificationPreference(NotificationPreference value) => _notificationPreferences.Add(value);
        public void AddReminderSetting(ReminderSetting value) => _reminderSettings.Add(value);
        public void AddCreatedConversation(Conversation value) => _createdConversations.Add(value);
        public void AddConversationParticipant(ConversationParticipant value) => _conversationParticipants.Add(value);
        public void AddSentMessage(Message value) => _sentMessages.Add(value);
        public void AddMessageReaction(MessageReaction value) => _messageReactions.Add(value);
        public void AddUserActive(UserActive value) => _userActives.Add(value);

    }
}
