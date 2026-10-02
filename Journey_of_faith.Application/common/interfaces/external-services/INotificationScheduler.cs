using Journey_of_faith.Application.usecases.notifications;

namespace Journey_of_faith.Application.common.interfaces;

public interface INotificationScheduler
{
    Task<ScheduledNotificationResponse> ScheduleAsync(
        ScheduleNotificationRequest request, CancellationToken cancellationToken = default);
    Task<ScheduledNotificationResponse?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
