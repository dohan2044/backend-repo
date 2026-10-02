using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities.catholic;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.entities.masslive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.interfaces
{
    public class ReminderSettingView
    {
        public bool MassReminderEnabled { get; set; }
        public int MinutesBefore { get; set; }
        public string? SpeechGender { get; set; }
        public double? SpeechSpeed { get; set; }
    }
    public interface IChurchRepository
    {
        Task AddAsync(MassType massType, CancellationToken cancellationToken = default);
        Task<bool> DeleteMassTypeAsync(int id, CancellationToken cancellationToken = default);

        Task AddAsync(Church church, CancellationToken cancellationToken = default);
        void Update(Church church);
        Task<bool> DeleteChurchAsync(int id, bool force = false, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<Church> churches, CancellationToken cancellationToken = default);

        Task AddAsync(Liturgy liturgy, CancellationToken cancellationToken = default);
        Task AddMassSchedulesAsync(IEnumerable<MassSchedule> massSchedules, CancellationToken cancellationToken = default);

        Task AddAsync(Diocese diocese, CancellationToken cancellationToken = default);
        void Update(Diocese diocese);
        Task<bool> DeleteDioceseAsync(int id, CancellationToken cancellationToken = default);

        Task FollowChurchAsync(UserChurch userChurch, CancellationToken cancellationToken = default);
        Task<bool> UnfollowChurchAsync(Guid userId, int churchId, CancellationToken cancellationToken = default);
        Task<ReminderSettingView> SaveReminderSettingAsync(Guid userId, bool isEnabled, int minutesBefore, string? speechGender, double? speechSpeed);
        Task AddAsync(DailyWord dailyWord, CancellationToken cancellationToken = default);
    }
}
