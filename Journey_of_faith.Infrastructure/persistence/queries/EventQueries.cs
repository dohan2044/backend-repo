using System.Data;
using System.Text.Json;
using Dapper;
using Journey_of_faith.Application.common.dtos.Events;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.events;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class EventQueries : BaseRepository, IEventQueries
{
    public EventQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<bool> EventExistsAsync(int eventId, CancellationToken cancellationToken = default)
        => ExistsAsync(EventTables.Event, "Id = @Id AND ISNULL(IsDeleted, 0) = 0", new { Id = eventId }, cancellationToken);

    public Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default)
        => ExistsAsync(EventTables.EventCategory, "Id = @Id", new { Id = categoryId }, cancellationToken);

    public Task<bool> CategoryNameExistsAsync(string categoryName, CancellationToken cancellationToken = default)
        => ExistsAsync(EventTables.EventCategory, "Name = @Name", new { Name = categoryName }, cancellationToken);

    public Task<IReadOnlyList<EventCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<EventCategoryDto>>(async connection =>
        {
            var categories = (await connection.QueryAsync<EventCategoryDto>(new CommandDefinition(
                "sp_GetCategoryEvent",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken))).ToList();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            foreach (var category in categories)
            {
                category.EventsList = string.IsNullOrWhiteSpace(category.Events)
                    ? []
                    : JsonSerializer.Deserialize<List<EventViewDto>>(category.Events, options) ?? [];
            }
            return categories;
        });

    public Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, Guid? userId, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
                "sp_GetEventDetails",
                new { EventId = eventId, UserId = userId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            var details = await multi.ReadSingleOrDefaultAsync<EventDetailsDto>();
            if (details is null) return null;
            details.Categories = (await multi.ReadAsync<EventCategoryDto>()).ToList();
            details.Images = (await multi.ReadAsync<EventImageDto>()).ToList();
            return details;
        });

    public Task<PagedResult<EventViewDto>> GetEventsAsync(EventFilterDto filter, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
                "sp_EventsPage",
                new { filter.Keyword, filter.PageIndex, filter.PageSize },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            var items = (await multi.ReadAsync<EventViewDto>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();
            return new PagedResult<EventViewDto>
            {
                Data = items,
                TotalCount = totalCount,
                Page = filter.PageIndex,
                PageSize = filter.PageSize
            };
        });

    public Task<bool> IsFollowingEventAsync(Guid userId, int eventId, CancellationToken cancellationToken = default)
        => ExistsAsync(EventTables.UserEvent, "UserId = @UserId AND EventId = @EventId", new { UserId = userId, EventId = eventId }, cancellationToken);

    public Task<IReadOnlyList<EventViewDto>> GetFollowedEventsAsync(
        Guid userId,
        DateTime? startFrom,
        DateTime? startTo,
        CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<EventViewDto>>(async connection =>
            (await connection.QueryAsync<EventViewDto>(new CommandDefinition(
                "sp_GetFollowedEvents",
                new { UserId = userId, StartFrom = startFrom, StartTo = startTo },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken))).ToList());

    public Task<IReadOnlyList<EventCommentDto>> GetCommentsAsync(int eventId, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<EventCommentDto>>(async connection =>
            (await connection.QueryAsync<EventCommentDto>(new CommandDefinition(
                $"""
                SELECT userAccount.Username, comment.Comment
                FROM [{_schemaName.Schema}].[{EventTables.EventComment}] comment
                LEFT JOIN [{_schemaName.Schema}].[User] userAccount ON userAccount.Id = comment.UserId
                WHERE comment.EventId = @EventId
                ORDER BY comment.CreatedTime
                """,
                new { EventId = eventId },
                cancellationToken: cancellationToken))).ToList());

    private Task<bool> ExistsAsync(string table, string predicate, object parameters, CancellationToken cancellationToken)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{table}] WHERE {predicate}) THEN 1 ELSE 0 END",
                parameters,
                cancellationToken: cancellationToken)) > 0);
}
