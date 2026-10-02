using Journey_of_faith.Application.common.interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Journey_of_faith.Infrastructure.identity.services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _context;
        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string UserId => _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier ?? ClaimTypes.Email)?.Value;
        public string GetRoleUserName => _context?.HttpContext?.User?.FindFirst("role")?.Value;
    }
}
