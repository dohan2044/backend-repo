using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.notifications
{
    public class NotificationPreference
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public bool MassReminder { get; set; }
        public bool FeastReminder { get; set; }
        public bool DailyWord { get; set; }
        public bool EventUpdates { get; set; }
        public bool FriendRequests { get; set; }
        public bool Messages { get; set; }
    }
}
