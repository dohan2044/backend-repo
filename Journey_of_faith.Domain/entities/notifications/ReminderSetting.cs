using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.notifications
{
    public class ReminderSetting
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int MinutesBefore { get; set; }
        public string? SpeechGender { get; set; }
        public float? SpeechSpeed { get; set; }
    }
}
