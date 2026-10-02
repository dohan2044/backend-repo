using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.events
{
    public class EventParticipant
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? RegisteredTime { get; set; }
        public Event Event { get; set; } = null!;
    }
}
