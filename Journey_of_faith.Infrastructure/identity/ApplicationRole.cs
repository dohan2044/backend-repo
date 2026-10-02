using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Journey_of_faith.Infrastructure.identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Descriptions {get; set;}
    }
}
