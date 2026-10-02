using Journey_of_faith.Application.common.dtos.church;
using Journey_of_faith.Domain.dtos;

namespace Journey_of_faith.Application.usecases.churchs;

public interface IChurchQueries
{
    Task<PagedResult<ChurchViewDto>> GetListAsync(QueryFilter filter);
    Task<IEnumerable<MassScheduleViewDto>> GetMassScheduleToday();
    Task<IEnumerable<ChurchViewDto>> GetListFollowedAsync(Guid id);
    Task<ChurchViewDto?> GetChurchByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<ChurchViewDto>> GetChurchesAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<LiturgyViewDto?> GetLiturgyTodayAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MassScheduleTodayDto>> GetMassScheduleTodayViewsAsync(
        bool nextDay = false,
        string? province = null,
        CancellationToken cancellationToken = default);
    Task<bool> DioceseExistsAsync(int dioceseId, CancellationToken cancellationToken = default);
    Task<bool> DioceseNameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<DioceseViewDto?> GetDioceseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DioceseViewDto>> GetAllDiocesesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ChurchListItemDto>> SearchChurchesAsync(string? keyword, int? dioceseId, Guid? userId, CancellationToken cancellationToken = default);
    Task<bool> ChurchExistsAsync(int churchId, CancellationToken cancellationToken = default);
    Task<bool> IsFollowingChurchAsync(Guid userId, int churchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChurchViewDto>> GetFollowedChurchesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ReminderSettingDto> GetReminderSettingAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<DailyWordViewDto?> GetDailyWordAsync(CancellationToken cancellationToken = default);
}
