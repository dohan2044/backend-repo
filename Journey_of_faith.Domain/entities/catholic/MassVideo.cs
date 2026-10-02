using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.catholic
{
    public class MassVideo : AuditableEntity
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ViewCount { get; set; }
    }
}
