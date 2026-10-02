using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;

public class CreateRoleCommand : IRequest<Guid>
{
    public string Name {get; set;}
    public string Description {get; set;}
}

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly IRoleRepository roleRepository;
    private readonly IRoleQueries roleQueries;
    public CreateRoleHandler(IRoleRepository roleRepository, IRoleQueries roleQueries)
    {
        this.roleRepository = roleRepository;
        this.roleQueries = roleQueries;
    }

    public async Task<Guid> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        if(await roleQueries.NameExistsAsync(command.Name, cancellationToken))
        {
            throw new ConfictException("Tên vai trò đã tồn tại");
        }
        var role = new Role
        {
            Name = command.Name,
            Descriptions = command.Description,
        };
        return await roleRepository.CreateAsync(role, cancellationToken);
    }
}
