using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.identity
{
    public class RefreshToken
    {
        public Guid Id { get; set;  }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpiresOnUtc { get; set; }
        public ApplicationUser User { get; set; }
    }
}
