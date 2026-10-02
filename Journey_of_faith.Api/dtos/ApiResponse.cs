namespace Journey_of_faith.Api.dtos
{
    public class ApiResponse<T> 
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
