using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;


public class ChangePasswordCommand : IRequest<Unit>
{
    public string CurrentPassword {get;set;}
    public string NewPassword {get; set;}
}