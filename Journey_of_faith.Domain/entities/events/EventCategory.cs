using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.events
{
    public class EventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<EventCategoryMapping> EventMappings { get; set; } = [];
    }
}
