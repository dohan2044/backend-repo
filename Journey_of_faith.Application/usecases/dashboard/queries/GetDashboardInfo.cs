using Journey_of_faith.Application.common.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.dashboard.queries;


public class GetDashboardQuery : IRequest<DashboardInfoDto>
{
    
}


public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, DashboardInfoDto>
{
    private readonly IDashboardQueries dashboardQueries;
    public GetDashboardHandler(IDashboardQueries dashboardQueries)
    {
        this.dashboardQueries = dashboardQueries;
    }

    public async Task<DashboardInfoDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        return await dashboardQueries.GetDashboardInfoAsync(cancellationToken);
    }
}
