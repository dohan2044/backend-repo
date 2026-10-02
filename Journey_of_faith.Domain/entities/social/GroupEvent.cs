using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.social
{
    public class GroupEvent
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public Guid UserId { get; set; }
        public int? ActionType { get; set; }
        public DateTime? ActionTime { get; set; }
        public Conversation Conversation { get; set; } = null!;
    }
}
