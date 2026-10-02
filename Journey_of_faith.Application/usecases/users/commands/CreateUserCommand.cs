using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.users.commands
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string RoleName {get; set; } = string.Empty;
    }
}
