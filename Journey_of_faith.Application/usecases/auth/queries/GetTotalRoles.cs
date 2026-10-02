using MediatR;

namespace Journey_of_faith.Application.usecases.auth.queries;

public class GetTotalUserPerRoleQuery: IRequest<Dictionary<string, int>>
{
    
}

public class GetTotalUserPerRoleHandler : IRequestHandler<GetTotalUserPerRoleQuery, Dictionary<string, int>>
{
    private readonly IRoleQueries roleQueries;
    public GetTotalUserPerRoleHandler(IRoleQueries roleQueries)
    {
        this.roleQueries = roleQueries;
    }


    public async Task<Dictionary<string, int>> Handle(GetTotalUserPerRoleQuery query, CancellationToken cancellationToken)
    {
        return await roleQueries.GetTotalUsersByRoleAsync(cancellationToken);
    }
}
