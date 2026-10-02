using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.entities.events;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.common;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.Json;

namespace Journey_of_faith.Infrastructure.repositories
{
    public class EventRepository : BaseRepository, IEventRepository
    {
        public EventRepository(IDbConnectionFactory dbConnection, IOptions<TableSchemaName> schemaName)
            : base(dbConnection, schemaName)
        {
        }

        public async Task<int> CreateCategoryAsync(string categoryName)
        {
            return await ExecuteAsync(async connection =>
                await connection.ExecuteScalarAsync<int>($@"
                    INSERT INTO [{_schemaName.Schema}].[{EventTables.EventCategory}] (Name)
                    OUTPUT inserted.Id
                    VALUES (@Name)
                ", new { Name = categoryName })
            );
        }

        public async Task<int> CreateEventAsync(string json)
        {
            return await ExecuteAsync(async connection =>
            {
                var paramter = new { JsonData = json };
                return await connection.ExecuteAsync(
                    "spCreateEventWithAnotherToping",
                    paramter,
                    commandType: CommandType.StoredProcedure
                );
            });
        }

        public async Task<bool> UpdateEventAsync(UpdateEventPayload payload)
        {
            return await ExecuteAsync(async connection =>
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using var transaction = connection.BeginTransaction();
                try
                {
                    var affectedRows = await connection.ExecuteAsync($@"
                        UPDATE [{_schemaName.Schema}].[{EventTables.Event}] SET
                            Title = @Title,
                            Description = @Description,
                            Location = @Location,
                            StartDate = @StartDate,
                            EndDate = @EndDate,
                            ImageUrl = @ImageUrl,
                            LastModifierUserId = @LastModifierUserId,
                            LastModificationTime = GETDATE()
                        WHERE Id = @Id AND IsDeleted = 0
                    ", new
                    {
                        payload.Id,
                        payload.Title,
                        payload.Description,
                        payload.Location,
                        payload.StartDate,
                        payload.EndDate,
                        payload.ImageUrl,
                        payload.LastModifierUserId
                    }, transaction);

                    if (affectedRows == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    if (payload.CategoryIds is not null)
                    {
                        await connection.ExecuteAsync($@"
                            DELETE FROM [{_schemaName.Schema}].[{EventTables.EventCategoryMapping}]
                            WHERE EventId = @EventId
                        ", new { EventId = payload.Id }, transaction);

                        var categoryIds = payload.CategoryIds.Distinct().ToList();
                        foreach (var categoryId in categoryIds)
                        {
                            await connection.ExecuteAsync($@"
                                INSERT INTO [{_schemaName.Schema}].[{EventTables.EventCategoryMapping}] (EventId, CategoryId)
                                VALUES (@EventId, @CategoryId)
                            ", new { EventId = payload.Id, CategoryId = categoryId }, transaction);
                        }
                    }

                    if (payload.ImageUrls is not null)
                    {
                        await connection.ExecuteAsync($@"
                            DELETE FROM [{_schemaName.Schema}].[{EventTables.EventImage}]
                            WHERE EventId = @EventId
                        ", new { EventId = payload.Id }, transaction);

                        var imageUrls = payload.ImageUrls
                            .Where(url => !string.IsNullOrWhiteSpace(url))
                            .Select(url => url.Trim())
                            .Distinct()
                            .ToList();

                        foreach (var imageUrl in imageUrls)
                        {
                            await connection.ExecuteAsync($@"
                                INSERT INTO [{_schemaName.Schema}].[{EventTables.EventImage}] (EventId, ImageUrl)
                                VALUES (@EventId, @ImageUrl)
                            ", new { EventId = payload.Id, ImageUrl = imageUrl }, transaction);
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            return await ExecuteAsync(async connection =>
                await connection.ExecuteAsync($@"
                    UPDATE [{_schemaName.Schema}].[{EventTables.Event}] SET
                        IsDeleted = 1,
                        DeletionTime = GETDATE(),
                        LastModificationTime = GETDATE()
                    WHERE Id = @EventId AND IsDeleted = 0
                ", new { EventId = eventId }) > 0
            );
        }

        public async Task<bool> FollowEventAsync(Guid userId, int eventId)
        {
            return await ExecuteAsync(async connection =>
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using var transaction = connection.BeginTransaction();
                try
                {
                    var exists = await connection.ExecuteScalarAsync<int>($@"
                        IF EXISTS (
                            SELECT 1
                            FROM [{_schemaName.Schema}].[{EventTables.UserEvent}]
                            WHERE UserId = @UserId
                              AND EventId = @EventId
                        ) SELECT 1 ELSE SELECT 0
                    ", new { UserId = userId, EventId = eventId }, transaction);

                    if (exists == 1)
                    {
                        transaction.Commit();
                        return false;
                    }

                    await connection.ExecuteAsync($@"
                        INSERT INTO [{_schemaName.Schema}].[{EventTables.UserEvent}] (UserId, EventId, FollowedAt)
                        VALUES (@UserId, @EventId, GETDATE())
                    ", new { UserId = userId, EventId = eventId }, transaction);

                    await connection.ExecuteAsync($@"
                        INSERT INTO [{_schemaName.Schema}].[{EventTables.EventFollower}] (EventId, UserId, FollowedTime)
                        VALUES (@EventId, @UserId, GETUTCDATE())
                    ", new { EventId = eventId, UserId = userId }, transaction);

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
        }

        public async Task<bool> UnfollowEventAsync(Guid userId, int eventId)
        {
            return await ExecuteAsync(async connection =>
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using var transaction = connection.BeginTransaction();
                try
                {
                    var deletedFromUserEvent = await connection.ExecuteAsync($@"
                        DELETE FROM [{_schemaName.Schema}].[{EventTables.UserEvent}]
                        WHERE UserId = @UserId AND EventId = @EventId
                    ", new { UserId = userId, EventId = eventId }, transaction);

                    await connection.ExecuteAsync($@"
                        DELETE FROM [{_schemaName.Schema}].[{EventTables.EventFollower}]
                        WHERE UserId = @UserId AND EventId = @EventId
                    ", new { UserId = userId, EventId = eventId }, transaction);

                    transaction.Commit();
                    return deletedFromUserEvent > 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
        }

        public async Task<bool> CreateEventCommentAsync(EventComment comment)
        {
            return await ExecuteAsync(async connection =>
            {
                await connection.ExecuteAsync(@"
                  Insert into [jcodepro_journey_of_faith].[EventComment] (EventId, UserId, Comment, CreatedTime)
                    Values(@EventId, @UserId, @Comment, @CreatedTime);
               ", new {EventId = comment.EventId, UserId = comment.UserId, Comment = comment.Comment, CreatedTime = comment.CreatedTime});
                return true;
            });
        }

    }

    public static class EventTables
    {
        public const string Event = "Event";
        public const string EventCategory = "EventCategory";
        public const string EventCategoryMapping = "EventCategoryMapping";
        public const string EventFollower = "EventFollower";
        public const string EventParticipant = "EventParticipant";
        public const string EventImage = "EventImage";
        public const string EventNotification = "EventNotification";
        public const string EventComment = "EventComment";
        public const string UserEvent = "UserEvent";
    }
}
