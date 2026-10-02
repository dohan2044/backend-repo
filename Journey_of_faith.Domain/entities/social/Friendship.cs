using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.social
{
    public class Friendship : AuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public string Status { get; set; } = string.Empty;   // Pending, Accepted, Rejected, Blocked
    }
}
