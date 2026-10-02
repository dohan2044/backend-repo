using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Journey_of_faith.Infrastructure.repositories;


public class GetDataRepository<TData, TEntity> : BaseRepository,  IGetOneToManyData<TData, TEntity>, IGetOneToOneData<TData, TEntity>
where TData : notnull
where TEntity : class
{
    private readonly ILogger<GetDataRepository<TData, TEntity>> _logger;

    public GetDataRepository(
        IDbConnectionFactory dbConnection,
        IOptions<TableSchemaName> options,
        ILogger<GetDataRepository<TData, TEntity>> logger) : base(dbConnection, options
    )
    {
        _logger = logger;
    }

    public async Task<Dictionary<TData, TEntity>> GetOneToOneDataAsync(string sql, IReadOnlyList<TData> ids, Func<TEntity, TData> keySelector, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        if(ids == null || !ids.Any())
        {
            _logger.LogDebug("Skipped one-to-one lookup for {EntityType} because no IDs were supplied", typeof(TEntity).Name);
            return new Dictionary<TData, TEntity>();
        }

        try
        {
            using var connection = _dbConnection.CreateConnection();
            var command = new CommandDefinition(sql, new { Ids = ids}, cancellationToken: cancellationToken);
            var data = (await connection.QueryAsync<TEntity>(command)).ToArray();
            _logger.LogInformation(
                "Loaded {ResultCount} {EntityType} record(s) for one-to-one lookup in {ElapsedMilliseconds} ms",
                data.Length,
                typeof(TEntity).Name,
                stopwatch.ElapsedMilliseconds);
            return data.ToDictionary(keySelector);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed one-to-one lookup for {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public async Task<Dictionary<TData, TEntity[]>> GetDataByIdsAsync(string sql,IReadOnlyList<TData> ids, Func<TEntity, TData> keySelector, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        if(ids.Count == 0 || !ids.Any())
        {
            _logger.LogDebug("Skipped one-to-many lookup for {EntityType} because no IDs were supplied", typeof(TEntity).Name);
            return new Dictionary<TData, TEntity[]>();
        }

        try
        {
            using var connection = _dbConnection.CreateConnection();
            var command = new CommandDefinition(sql, new {Ids = ids}, cancellationToken: cancellationToken);
            var data = (await connection.QueryAsync<TEntity>(command)).ToArray();

            var lookup = data.ToLookup(keySelector: keySelector);
            var result = ids.Distinct()
                .ToDictionary(
                    id => id,
                    id => lookup[id].ToArray()
                );

            _logger.LogInformation(
                "Loaded {ResultCount} {EntityType} record(s) for one-to-many lookup in {ElapsedMilliseconds} ms",
                data.Length,
                typeof(TEntity).Name,
                stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed one-to-many lookup for {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }
}
