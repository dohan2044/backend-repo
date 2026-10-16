namespace Journey_of_faith.Application.usecases.students;


public interface IStudentQueries
{
    Task<bool> ExistsGroupNameAsync(string groupName, CancellationToken cancellationToken = default);
    Task<bool> ExistsGroupIdAsync(int groupId, CancellationToken cancellationToken = default);
}