using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;


public class AddPermissionCommand : IRequest<bool>
{
    public string RoleName {get; set;}
    public List<string> Permissions {get; set;}
}