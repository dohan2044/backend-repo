using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.users.commands;

public class IncrementCommand : IRequest<Unit>
{
    public int Score {get; set;}
}


public class IncrementHandler : IRequestHandler<IncrementCommand, Unit>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IUserRepository userRepository;
    private readonly IUserQueries userQueries;
    public IncrementHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IUserQueries userQueries
    )
    {
        this.currentUserService = currentUserService;
        this.userRepository = userRepository;
        this.userQueries = userQueries;
    }

    public async Task<Unit> Handle(IncrementCommand command, CancellationToken cancellationToken = default)
    {
        if(!Guid.TryParse(currentUserService.UserId, out Guid userId)) 
            throw new UnauthorizationException("Vui lòng đăng nhập");
        
        if(userQueries.GetUserAsync(userId, cancellationToken: cancellationToken) is null) 
            throw new NotFoundException("Vui lòng đăng nhập");

        await userRepository.IncrementAsync(userId, command.Score, cancellationToken);
        return Unit.Value;
    }
}