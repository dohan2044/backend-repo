using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.social
{
    public class GroupMember
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Guid UserId { get; set; }
        public int? RoleId { get; set; }
        public DateTime? JoinedTime { get; set; }
        public Group Group { get; set; } = null!;
    }
}
