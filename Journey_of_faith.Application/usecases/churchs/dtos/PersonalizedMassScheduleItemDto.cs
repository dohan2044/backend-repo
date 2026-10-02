namespace Journey_of_faith.Application.usecases.churchs.dtos
{
    public class PersonalizedMassScheduleItemDto
    {
        public int MassScheduleId { get; set; }
        public int ChurchId { get; set; }
        public string ChurchName { get; set; } = string.Empty;
        public string? ChurchAddress { get; set; }
        public bool? IsFixed { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TimeSpan Time { get; set; }
        public int? MassTypeId { get; set; }
        public string? MassTypeName { get; set; }

        public DateTime? MassStartAt { get; set; }
        public bool IsReminderEnabled { get; set; }
        public int MinutesBefore { get; set; }
        public DateTime? ReminderAt { get; set; }
    }
}
