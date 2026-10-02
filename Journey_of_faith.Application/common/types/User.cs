public record UserView(string username, string email, string name,
        string avatar, IList<string> role, List<string>? church);