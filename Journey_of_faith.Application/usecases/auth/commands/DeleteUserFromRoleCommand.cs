using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;


public class DeleteUserFromRoleCommand : IRequest<bool>
{
    public Guid userId {get; set;}
    public required string RoleName {get; set;}
}
public class DeleteUserFromRoleHandler : IRequestHandler<DeleteUserFromRoleCommand, bool>
{
    private readonly IRoleRepository roleRepository;
    public DeleteUserFromRoleHandler(IRoleRepository roleRepository)
    {
        this.roleRepository = roleRepository;
    }


    public async Task<bool> Handle(DeleteUserFromRoleCommand command, CancellationToken cancellationToken)
    {
        return await roleRepository.RemoveUserFromRole(command.userId, command.RoleName);
    }
}