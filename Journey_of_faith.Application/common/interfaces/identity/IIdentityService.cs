using Journey_of_faith.Domain.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.common.interfaces
{
    public interface IIdentityService
    {
        Task<bool> CreateAsync(User user, string? roleName);
        Task<bool> ExistsEmail(string email);

        Task<User?> GetUserByIdAsync(Guid id);
        Task<bool> UpdateRoleAsync(Guid Id, string oldRoleName, string newRoleName);

    }
}
