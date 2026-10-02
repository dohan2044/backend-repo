using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.events
{
    public class EventImage
    {
        public long Id { get; set; }
        public int EventId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Event Event { get; set; } = null!;
    }
}
