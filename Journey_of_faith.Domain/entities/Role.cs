using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Descriptions {get; set;}
    }
}
