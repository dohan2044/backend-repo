using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.events
{
    public class EventCategoryMapping
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public Event Event { get; set; } = null!;
        public EventCategory Category { get; set; } = null!;
    }
}
