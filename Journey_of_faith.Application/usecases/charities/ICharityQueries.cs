namespace Journey_of_faith.Application.usecases.charities;

public interface ICharityQueries
{
    Task<bool> CharityExistsAsync(int charityId, CancellationToken cancellationToken = default);
    Task<bool> RegistrationExistsAsync(int charityId, Guid userId, CancellationToken cancellationToken = default);
}
