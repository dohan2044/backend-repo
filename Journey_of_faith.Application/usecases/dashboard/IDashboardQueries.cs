using Journey_of_faith.Application.common.dtos;

namespace Journey_of_faith.Application.usecases.dashboard;

public interface IDashboardQueries
{
    Task<DashboardInfoDto> GetDashboardInfoAsync(CancellationToken cancellationToken = default);
}
