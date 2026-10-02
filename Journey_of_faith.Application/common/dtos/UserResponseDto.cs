namespace Journey_of_faith.Application.common.dtos;

public class UserResponseDto
{
    public Guid Id {get; set;}
    public string UserName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Role {get; set;} = string.Empty;
    public string Avatar {get; set;} = string.Empty;
    public bool? IsDeleted {get; set;}
    public int Score {get; set;} = 0;
    public int DayStreak {get; set;} = 0;
}
