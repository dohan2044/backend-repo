namespace Journey_of_faith.Application.usecases.students;


public interface IStudentQueries
{
    Task<bool> ExistsGroupNameAsync(string groupName);
}