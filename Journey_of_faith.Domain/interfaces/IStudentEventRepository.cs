using Domain.Entities;

namespace Journey_of_faith.Domain.interfaces;


public interface IStudentEventRepository
{
    void AddStudentEvent(StudentEvent studentEvent);
    void UpdateStudentEvent(StudentEvent studentEvent);
    void DeleteStudentEvent(StudentEvent studentEvent);
    StudentEvent? GetStudentEventById(int studentEventId);
    IEnumerable<StudentEvent> GetStudentEvents(int page = 1, int pageSize = 15, string? textSearch = null);

    void RegisterStudentEvent(StudentEventRegistration studentEventRegistration);
}