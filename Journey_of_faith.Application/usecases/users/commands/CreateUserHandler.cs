using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.users.commands
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IIdentityService _services;
        public CreateUserHandler(IIdentityService services)
        {
            _services = services;
        }

        public async Task<bool> Handle(CreateUserCommand command, CancellationToken token)
        {
            if(await _services.ExistsEmail(command.Email))
            {
                throw new ConfictException($"Email {command.Email} đã đươc sử dụng, vui lòng nhập email khác");
            }

            var user = User.Create(command.Username, command.Password, command.Email);

            await _services.CreateAsync(user, command.RoleName);
            return true;
        }
    }
}
