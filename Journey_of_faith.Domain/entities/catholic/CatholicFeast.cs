using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.catholic
{
    public class CatholicFeast : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime FeastDate { get; set; }
        public bool IsFixed { get; set; }
        public string? Description { get; set; }
    }
}
