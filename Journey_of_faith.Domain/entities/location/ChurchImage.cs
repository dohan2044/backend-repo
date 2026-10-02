namespace Journey_of_faith.Domain.entities.location;

public class ChurchImage
{
    public int Id {get; set;}
    public int ChurchId {get; set;}
    public string ImageName {get; set;}
    public Guid CreatedUser {get; set;}
    public DateTime CreatedAt {get; set;}
    public Church Church { get; set; } = null!;
}
