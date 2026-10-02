using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.masslive
{
    public class MassType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<MassSchedule> MassSchedules { get; set; } = [];
    }
}
