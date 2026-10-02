using Journey_of_faith.Application.common.dtos.Events;
using Journey_of_faith.Domain.dtos;

namespace Journey_of_faith.Application.usecases.events;

public interface IEventQueries
{
    Task<bool> EventExistsAsync(int eventId, CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<bool> CategoryNameExistsAsync(string categoryName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, Guid? userId, CancellationToken cancellationToken = default);
    Task<PagedResult<EventViewDto>> GetEventsAsync(EventFilterDto filter, CancellationToken cancellationToken = default);
    Task<bool> IsFollowingEventAsync(Guid userId, int eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventViewDto>> GetFollowedEventsAsync(Guid userId, DateTime? startFrom, DateTime? startTo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventCommentDto>> GetCommentsAsync(int eventId, CancellationToken cancellationToken = default);
}
