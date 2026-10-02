using System.Data;
using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.entities.catholic;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.entities.masslive;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.context;
using Journey_of_faith.Infrastructure.common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.repositories;

public sealed class ChurchRepository : BaseRepository, IChurchRepository
{
    private const int DefaultReminderMinutes = 30;
    private readonly ApplicationDbContext _dbContext;
    private readonly string _schema;

    public ChurchRepository(
        IDbConnectionFactory dbConnection,
        ApplicationDbContext dbContext,
        IOptions<TableSchemaName> options)
        : base(dbConnection)
    {
        _dbContext = dbContext;
        _schema = options.Value.Schema;
    }

    public async Task AddAsync(MassType massType, CancellationToken cancellationToken = default)
        => await _dbContext.MassTypes.AddAsync(massType, cancellationToken);

    public async Task<bool> DeleteMassTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        var massType = await _dbContext.MassTypes.FindAsync([id], cancellationToken);
        if (massType is null) return false;

        _dbContext.MassTypes.Remove(massType);
        return true;
    }

    public async Task AddAsync(Church church, CancellationToken cancellationToken = default)
        => await _dbContext.Churches.AddAsync(church, cancellationToken);

    public void Update(Church church) => _dbContext.Churches.Update(church);

    public async Task<bool> DeleteChurchAsync(
        int id,
        bool force = false,
        CancellationToken cancellationToken = default)
    {
        var church = await _dbContext.Churches
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (church is null) return false;

        if (force)
        {
            _dbContext.Churches.Remove(church);
        }
        else
        {
            church.IsDeleted = true;
            church.DeletionTime = DateTime.UtcNow;
        }

        return true;
    }

    public async Task AddRangeAsync(
        IEnumerable<Church> churches,
        CancellationToken cancellationToken = default)
        => await _dbContext.Churches.AddRangeAsync(churches, cancellationToken);

    public async Task AddAsync(Liturgy liturgy, CancellationToken cancellationToken = default)
        => await _dbContext.Liturgy.AddAsync(liturgy, cancellationToken);

    public async Task AddMassSchedulesAsync(
        IEnumerable<MassSchedule> massSchedules,
        CancellationToken cancellationToken = default)
        => await _dbContext.MassSchedules.AddRangeAsync(massSchedules, cancellationToken);

    public async Task AddAsync(Diocese diocese, CancellationToken cancellationToken = default)
        => await _dbContext.Dioceses.AddAsync(diocese, cancellationToken);

    public void Update(Diocese diocese) => _dbContext.Dioceses.Update(diocese);

    public async Task<bool> DeleteDioceseAsync(int id, CancellationToken cancellationToken = default)
    {
        var diocese = await _dbContext.Dioceses
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (diocese is null) return false;

        diocese.IsDeleted = true;
        diocese.DeletionTime = DateTime.UtcNow;
        return true;
    }

    public async Task FollowChurchAsync(
        UserChurch userChurch,
        CancellationToken cancellationToken = default)
        => await _dbContext.UserChurches.AddAsync(userChurch, cancellationToken);

    public async Task<bool> UnfollowChurchAsync(
        Guid userId,
        int churchId,
        CancellationToken cancellationToken = default)
    {
        var userChurch = await _dbContext.UserChurches
            .FirstOrDefaultAsync(
                item => item.UserId == userId && item.ChurchId == churchId,
                cancellationToken);

        if (userChurch is null) return false;

        _dbContext.UserChurches.Remove(userChurch);
        return true;
    }

    public async Task<ReminderSettingView> SaveReminderSettingAsync(
        Guid userId,
        bool isEnabled,
        int minutesBefore,
        string? speechGender,
        double? speechSpeed)
    {
        return await ExecuteAsync(async connection =>
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                var preferenceId = await connection.ExecuteScalarAsync<int?>(
                    $"""
                    SELECT TOP 1 Id
                    FROM [{_schema}].[{TableTopicChurch.NotificationPreference}]
                    WHERE UserId = @UserId
                    ORDER BY Id DESC
                    """,
                    new { UserId = userId }, transaction);

                if (preferenceId.HasValue)
                {
                    await connection.ExecuteAsync(
                        $"""
                        UPDATE [{_schema}].[{TableTopicChurch.NotificationPreference}]
                        SET MassReminder = @MassReminder
                        WHERE Id = @Id
                        """,
                        new { Id = preferenceId.Value, MassReminder = isEnabled }, transaction);
                }
                else
                {
                    await connection.ExecuteAsync(
                        $"""
                        INSERT INTO [{_schema}].[{TableTopicChurch.NotificationPreference}]
                            (UserId, MassReminder, FeastReminder, DailyWord, EventUpdates, FriendRequests, Messages)
                        VALUES (@UserId, @MassReminder, 0, 0, 0, 0, 0)
                        """,
                        new { UserId = userId, MassReminder = isEnabled }, transaction);
                }

                var settingId = await connection.ExecuteScalarAsync<int?>(
                    $"""
                    SELECT TOP 1 Id
                    FROM [{_schema}].[{TableTopicChurch.ReminderSetting}]
                    WHERE UserId = @UserId
                    ORDER BY Id DESC
                    """,
                    new { UserId = userId }, transaction);

                var normalizedMinutes = minutesBefore > 0 ? minutesBefore : DefaultReminderMinutes;

                if (settingId.HasValue)
                {
                    await connection.ExecuteAsync(
                        $"""
                        UPDATE [{_schema}].[{TableTopicChurch.ReminderSetting}]
                        SET MinutesBefore = @MinutesBefore,
                            SpeechGender = @SpeechGender,
                            SpeechSpeed = @SpeechSpeed
                        WHERE Id = @Id
                        """,
                        new
                        {
                            Id = settingId.Value,
                            MinutesBefore = normalizedMinutes,
                            SpeechGender = speechGender,
                            SpeechSpeed = speechSpeed
                        }, transaction);
                }
                else
                {
                    await connection.ExecuteAsync(
                        $"""
                        INSERT INTO [{_schema}].[{TableTopicChurch.ReminderSetting}]
                            (UserId, MinutesBefore, SpeechGender, SpeechSpeed)
                        VALUES (@UserId, @MinutesBefore, @SpeechGender, @SpeechSpeed)
                        """,
                        new
                        {
                            UserId = userId,
                            MinutesBefore = normalizedMinutes,
                            SpeechGender = speechGender,
                            SpeechSpeed = speechSpeed
                        }, transaction);
                }

                transaction.Commit();
                return new ReminderSettingView
                {
                    MassReminderEnabled = isEnabled,
                    MinutesBefore = normalizedMinutes,
                    SpeechGender = speechGender,
                    SpeechSpeed = speechSpeed
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });
    }

    public async Task AddAsync(DailyWord dailyWord, CancellationToken cancellationToken = default)
        => await _dbContext.DailyWords.AddAsync(dailyWord, cancellationToken);
}

public static class TableTopicChurch
{
    public const string Church = "Church";
    public const string MassSchedule = "MassSchedule";
    public const string Diocese = "Diocese";
    public const string UserChurch = "UserChurch";
    public const string NotificationPreference = "NotificationPreference";
    public const string ReminderSetting = "ReminderSetting";
    public const string Liturgy = "Liturgy";
    public const string ChurchImages = "ChurchImages";
}
