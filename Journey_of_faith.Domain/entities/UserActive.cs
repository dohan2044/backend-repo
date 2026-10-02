namespace Journey_of_faith.Domain.entities;


public class UserActive
{
    public int Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public bool Status {get; set;}
    public string? ActiveLocation {get; set;}
    public DateTime Timespan {get; set;}
}
