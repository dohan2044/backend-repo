namespace Journey_of_faith.Domain.entities.events;

public class NotificationLogs
{
    public int Id {get; set;}
    public Guid UserId {get; set;}
    public string Title {get; set;}
    public string Body {get; set;}
    public string? TargetTopic {get; set;}
    public DateTime? SendAt {get; set;}
}