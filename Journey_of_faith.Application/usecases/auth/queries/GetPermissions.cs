using Journey_of_faith.Application.common.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.queries;

public class GetPermissionsQuery : IRequest<IReadOnlyList<RolePermissionDto>>
{

}


public class GetPermissionsHandler : IRequestHandler<GetPermissionsQuery, IReadOnlyList<RolePermissionDto>>
{
    private readonly IRoleQueries roleQueries;
    public GetPermissionsHandler(IRoleQueries roleQueries)
    {
        this.roleQueries = roleQueries;
    }

    public async Task<IReadOnlyList<RolePermissionDto>> Handle(GetPermissionsQuery query, CancellationToken cancellationToken)
    {
        return await roleQueries.GetPermissionsAsync(cancellationToken);
    }
}
