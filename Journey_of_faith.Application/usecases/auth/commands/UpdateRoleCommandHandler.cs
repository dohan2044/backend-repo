using FluentValidation;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;


public class UpDateRoleCommand : IRequest<bool>
{
    public string RoleId {get; set;}
    public string? RoleName {get; set;}
    public string? Description {get; set;}
}

public class UpdateRoleValidator : AbstractValidator<UpDateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(e => e.RoleId).NotEmpty().WithMessage("RoleId is not empty.");
    }
}


public class UpdateRoleHandler : IRequestHandler<UpDateRoleCommand, bool>
{
    private readonly IRoleRepository roleRepository;
    public UpdateRoleHandler(IRoleRepository roleRepository)
    {
        this.roleRepository = roleRepository;
    }


    public async Task<bool> Handle(UpDateRoleCommand command, CancellationToken cancellationToken)
    {
        return await roleRepository.UpdateRoleAsync(command.RoleId, new Role {Name = command.RoleName, Descriptions = command.Description});
    }
}