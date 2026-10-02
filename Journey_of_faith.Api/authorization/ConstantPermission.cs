namespace Journey_of_faith.Api.authorization;
public static class Permissions
{
    public static class Event
    {
        public const string VIEW = "EVENTS_VIEW";
        public const string CREATE = "EVENTS_CREATE";
        public const string USER_CREATE = "EVENTS_CREATE";
        public const string USER_DELETE = "EVENTS_DELETE";
        public const string UPDATE = "EVENTS_UPDATE";
        public const string DELETE = "EVENTS_DELETE";
        public const string CREATE_CATEGORY = "EVENTS_CATEGORY_CREATE";
        public const string VIEW_CATEGORY = "EVENTS_CATEGORY_VIEW";
    }
    public static class Quizes
    {
        public const string CREATE = "QUIZZES_CREATE";
        public const string EDIT = "QUIZZES_EDIT";
        public const string VIEW = "QUIZZES_VIEW";
        public const string DELETE = "QUIZZES_DELETE";
        public const string SUBMIT = "QUIZZES_VIEW";

    }
    public static class Roles
    {
        public const string VIEW = "ROLES_VIEW";
        public const string CREATE = "ROLES_CREATE";
        public const string EDIT = "ROLES_EDIT";
        public const string DELETE = "ROLES_DELETE";
    }

    public static class Questions
    {
        public const string VIEW = "QUESTIONS_VIEW";
        public const string CREATE = "QUESTIONS_CREATE";
        public const string Edit = "QUESTIONS_EDIT";
        public const string DELETE = "QUESTIONS_DELETE";
    }

    public static class Arena
    {
        public const string VIEW = "ARENA_VIEW";
        public const string MANAGE = "ARENA_MANAGE"; // Admin: create/update/delete levels & questions
    }
}