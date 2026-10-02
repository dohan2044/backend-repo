using MediatR;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Application.common.dtos;
namespace Journey_of_faith.Application.usecases.users.queries;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResult<UserResponseDto>>
{
    private readonly IUserQueries _userQueries;
    public GetUsersHandler(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }


    public async Task<PagedResult<UserResponseDto>> Handle(GetUsersQuery query, CancellationToken token)
    {
        return await _userQueries.GetUsersAsync(query.Page, query.PageSize, query.Search, token);
    }
}
