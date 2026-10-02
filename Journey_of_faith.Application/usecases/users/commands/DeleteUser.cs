using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.users.commands;


public class DeleteUserCommand : IRequest<bool>
{
    public string Id {get; set;}
}


public class DeleteUserHandler : IRequestHandler<DeleteUserCommand,bool>
{
    private readonly IUserRepository userRepository;
    public DeleteUserHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }


    public async Task<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if(command.Id == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return await userRepository.DeleteUserAsync(Guid.Parse(command.Id));
    }
}