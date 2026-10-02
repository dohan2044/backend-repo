using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;

public class AddPermissionHandler : IRequestHandler<AddPermissionCommand, bool>
{
    private readonly IRoleRepository roleRepository;
    public AddPermissionHandler(IRoleRepository roleRepository)
    {
        this.roleRepository = roleRepository;
    }

    public async Task<bool> Handle(AddPermissionCommand command, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(command.RoleName))
        {
            throw new BadRequestException("Tên vai trò không được để trống");
        }

        if(!command.Permissions.Any())
        {
            throw new BadRequestException("Danh sách vai trò trống");
        }

        return await roleRepository.AddPermissionForRole(command.RoleName, command.Permissions);
    }
}