using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;

public class DeleteRoleCommand : IRequest<bool>
{
    public string RoleName {get; set;}
}


public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, bool>
{
    private readonly IRoleRepository roleRepository;
    public DeleteRoleHandler(IRoleRepository roleRepository) {
        this.roleRepository = roleRepository;
    }


    public async Task<bool> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(command.RoleName))
        {
            throw new BadRequestException($"RoleName is not empty: {nameof(command.RoleName)}");
        }

        return await roleRepository.DeleteRoleAsync(command.RoleName);
    }
}