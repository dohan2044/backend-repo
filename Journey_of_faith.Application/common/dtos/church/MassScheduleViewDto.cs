namespace Journey_of_faith.Application.common.dtos.church;

public class MassScheduleViewDto
{
    public int Id {get; set;}
    public int ChurchId {get; set;}
    public string? ChurchName {get; set;}
    public string MassName {get; set;} = string.Empty;
    public string Time {get; set;} = string.Empty;
    public DateTime? Date { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsFixed { get; set; }
    public int? MassTypeId { get; set; }
    public string? MassTypeName { get; set; }
    public LiturgyViewDto Liturgy {get; set;} = new();
}
