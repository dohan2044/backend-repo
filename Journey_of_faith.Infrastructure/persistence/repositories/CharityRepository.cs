using Domain.Entities;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace Journey_of_faith.Infrastructure.persistence.repositories;

public sealed class CharityRepository(ApplicationDbContext context) : ICharityRepository
{
    public void AddCharity(CharityActivity charity)
        => context.CharityActivities.Add(charity);

    public void UpdateCharity(CharityActivity charity)
        => context.CharityActivities.Update(charity);

    public void DeleteCharity(CharityActivity charity)
    {
        charity.IsDeleted = true;
        charity.DeletionTime = DateTime.UtcNow;
        context.CharityActivities.Update(charity);
    }

    public CharityActivity? GetCharityById(int charityId)
        => context.CharityActivities
            .Include(charity => charity.Registrations)
            .FirstOrDefault(charity => charity.Id == charityId && charity.IsDeleted != true);

    public IEnumerable<CharityActivity> GetCharities(int page = 1, int pageSize = 15, string? textSearch = null)
    {
        ValidatePage(page, pageSize);

        var query = context.CharityActivities
            .AsNoTracking()
            .Where(charity => charity.IsDeleted != true);

        if (!string.IsNullOrWhiteSpace(textSearch))
        {
            var search = textSearch.Trim();
            query = query.Where(charity =>
                charity.Title.Contains(search) || charity.Location.Contains(search));
        }

        return query
            .OrderBy(charity => charity.StartAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public void RegistrationChatiry(CharityRegistration charityRegistration)
    {
        var charity = GetCharityById(charityRegistration.ActivityId)
            ?? throw new InvalidOperationException(
                $"Charity activity {charityRegistration.ActivityId} was not found.");

        charity.AddRegistration(charityRegistration);
    }

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
    }
}
