using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.users.queries;

public class GetMeQuery : IRequest<UserResponseDto>
{
    
}

public class GetMeHandler : IRequestHandler<GetMeQuery, UserResponseDto>
{
    private readonly IAuthService authService;
    public GetMeHandler(IAuthService authService)
    {
        this.authService = authService;
    }

    public async Task<UserResponseDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await authService.GetMe();
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.Username,
            Email = user.Email,
            Role = user.Role,
            Avatar = user.Avatar ?? string.Empty,
            IsDeleted = user.IsDeleted,
            Score = user.Score,
            DayStreak = user.DayStreak
            
        };
    }
}
