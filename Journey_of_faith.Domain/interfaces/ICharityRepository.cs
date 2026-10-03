using Domain.Entities;

namespace Journey_of_faith.Domain.interfaces;

public interface ICharityRepository
{

    /// <summary>
    /// Adds a new charity activity to the repository.
    /// </summary>
    /// <param name="charity"></param>
    void AddCharity(CharityActivity charity);
    void UpdateCharity(CharityActivity charity);
    void DeleteCharity(CharityActivity charity);
    CharityActivity? GetCharityById(int charityId);
    IEnumerable<CharityActivity> GetCharities(int page = 1, int pageSize = 15, string? textSearch = null);

    /// <summary>
    /// Registers a new charity registration.
    /// </summary>
    /// <param name="charityRegistration"></param>
    void RegistrationChatiry(CharityRegistration charityRegistration);
    
}