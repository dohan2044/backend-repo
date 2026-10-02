namespace Journey_of_faith.Application.common.dtos.church;

public class LiturgyViewDto
{
    public int Id {get; set;}
    public int MassScheduleId {get; set;}
    public string ReadingOne {get; set;} = string.Empty;
    public string ResponsorialPsalm {get; set;} = string.Empty;
    public string Gospel {get; set;} = string.Empty;
    public string EndWord {get; set;} = string.Empty;
    public DateTime DateActive { get; set; }
}
