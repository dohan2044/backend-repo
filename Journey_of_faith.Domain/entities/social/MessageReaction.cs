using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.social
{
    public class MessageReaction
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public Guid UserId { get; set; }
        public string? Reaction { get; set; }   // 👍 ❤️ 😂 ...
        public Message Message { get; set; } = null!;
    }
}
