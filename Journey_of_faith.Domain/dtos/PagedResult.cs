namespace Journey_of_faith.Domain.dtos;


public class PagedResult<T> where T : class
{
    public List<T> Data {get; set;} = new List<T>();
    public int Page {get; set;}
    public int PageSize {get; set;}
    public int TotalCount {get; set;}
}