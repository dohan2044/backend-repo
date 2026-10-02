using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities
{
    public class UserChurch
    {
        public Guid UserId { get; set; }
        public int ChurchId { get; set; }
        public location.Church Church { get; set; } = null!;
    }
}
