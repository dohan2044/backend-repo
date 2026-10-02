using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities;
using MediatR;

namespace Journey_of_faith.Application.usecases.users.queries;

public class GetUsersQuery : IRequest<PagedResult<UserResponseDto>>
{
    public int Page {get; set;}
    public int PageSize {get; set;}
    public string? Search {get; set;}
}