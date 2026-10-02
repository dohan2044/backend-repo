namespace Journey_of_faith.Application.common.dtos.church;

public class QueryFilter
{
    public string? ChurchName {get; set;}
    public string? Province {get; set;}
    public string? Ward {get; set;}
    public string? Time {get; set;}
    public int? Page {get; set;} = 1;
    public int? PageSize {get; set;} = 25;
}