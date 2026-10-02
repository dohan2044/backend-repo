using Journey_of_faith.Application.common.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.users.queries;

public class GetUserDetailsQuery : IRequest<UserResponseDto?>
{
    public Guid Id {get; set;}
}
