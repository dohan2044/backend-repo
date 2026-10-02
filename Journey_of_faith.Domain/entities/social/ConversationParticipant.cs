using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.social
{
    public class ConversationParticipant
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public Guid UserId { get; set; }
        public int? RoleId { get; set; }
        public DateTime? JoinedTime { get; set; }
        public long? LastReadMessageId { get; set; }
        public bool? IsMuted { get; set; }
        public bool? IsRemoved { get; set; }
        public Conversation Conversation { get; set; } = null!;
    }
}
