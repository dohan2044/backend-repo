using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Application.common.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.queries;

public class GetRolesQuery : IRequest<PagedResult<RoleViewDto>>
{
    public int Page {get; set;}
    public int PageSize {get; set;}
    public string? Search {get; set;}
}



public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PagedResult<RoleViewDto>>
{
    private readonly IRoleQueries roleQueries;
    public GetRolesQueryHandler(IRoleQueries roleQueries)
    {
        this.roleQueries = roleQueries;
    }

    public async Task<PagedResult<RoleViewDto>> Handle(GetRolesQuery query, CancellationToken cancellationToken)
    {
        return await roleQueries.GetRolesAsync(query.Page, query.PageSize, query.Search, cancellationToken);
    }
}
