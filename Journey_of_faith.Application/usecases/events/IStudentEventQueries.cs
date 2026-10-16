namespace Journey_of_faith.Application.usecases.events;

public interface IStudentEventQueries
{
    Task<bool> StudentEventExistsAsync(int studentEventId, CancellationToken cancellationToken = default);
    Task<bool> RegistrationExistsAsync(int studentEventId, Guid userId, CancellationToken cancellationToken = default);
}
