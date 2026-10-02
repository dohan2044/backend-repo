using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.social
{
    public class Group : AuditableEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public string? GroupType { get; set; }   // Church, Prayer, Community
        public string? Privacy { get; set; }

        private readonly List<GroupMember> _groupMembers = new();
        private readonly List<Conversation> _conversations = new();

        public IReadOnlyCollection<GroupMember> Members => _groupMembers.AsReadOnly();
        public IReadOnlyCollection<Conversation> Conversations => _conversations.AsReadOnly();
    }
}
