using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.notifications
{
    public class DeviceToken
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string? Platform { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
