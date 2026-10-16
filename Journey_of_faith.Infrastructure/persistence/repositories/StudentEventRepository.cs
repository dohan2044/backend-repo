using Domain.Entities;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace Journey_of_faith.Infrastructure.persistence.repositories;

public sealed class StudentEventRepository(ApplicationDbContext context) : IStudentEventRepository
{
    public void AddStudentEvent(StudentEvent studentEvent)
        => context.StudentEvents.Add(studentEvent);

    public void UpdateStudentEvent(StudentEvent studentEvent)
        => context.StudentEvents.Update(studentEvent);

    public void DeleteStudentEvent(StudentEvent studentEvent)
    {
        studentEvent.IsDeleted = true;
        studentEvent.DeletionTime = DateTime.UtcNow;
        context.StudentEvents.Update(studentEvent);
    }

    public StudentEvent? GetStudentEventById(int studentEventId)
        => context.StudentEvents
            .Include(studentEvent => studentEvent.Registrations)
            .FirstOrDefault(studentEvent =>
                studentEvent.Id == studentEventId && studentEvent.IsDeleted != true);

    public IEnumerable<StudentEvent> GetStudentEvents(int page = 1, int pageSize = 15, string? textSearch = null)
    {
        ValidatePage(page, pageSize);

        var query = context.StudentEvents
            .AsNoTracking()
            .Where(studentEvent => studentEvent.IsDeleted != true);

        if (!string.IsNullOrWhiteSpace(textSearch))
        {
            var search = textSearch.Trim();
            query = query.Where(studentEvent =>
                studentEvent.Title.Contains(search) || studentEvent.Location.Contains(search));
        }

        return query
            .OrderBy(studentEvent => studentEvent.StartAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public void RegisterStudentEvent(StudentEventRegistration studentEventRegistration)
    {
        var studentEvent = GetStudentEventById(studentEventRegistration.EventId)
            ?? throw new InvalidOperationException(
                $"Student event {studentEventRegistration.EventId} was not found.");

        studentEvent.AddRegistration(studentEventRegistration);
    }

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
    }
}
