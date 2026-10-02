using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.social
{
    public class Message
    {
        public long Id { get; set; }
        public Guid FromUserId { get; set; }
        public long ConversationId { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public int? MessageType { get; set; }   // 0 Text, 1 Image, 2 File, 3 System
        public DateTime? LastModificationTime { get; set; }
        public bool? IsDeleted { get; set; }
        public Conversation Conversation { get; set; } = null!;

        private readonly List<MessageAttachment> _attachments = new();
        private readonly List<MessageReaction> _reactions = new();
        private readonly List<MessageStatus> _statuses = new();

        public IReadOnlyCollection<MessageAttachment> Attachments => _attachments.AsReadOnly();
        public IReadOnlyCollection<MessageReaction> Reactions => _reactions.AsReadOnly();
        public IReadOnlyCollection<MessageStatus> Statuses => _statuses.AsReadOnly();
    }
}
