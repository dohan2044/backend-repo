using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Journey_of_faith.Infrastructure.context;

public sealed class LoggingSaveChangesInterceptor(
    ILogger<LoggingSaveChangesInterceptor> logger) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var changedEntities = eventData.Context?.ChangeTracker.Entries()
            .Count(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted) ?? 0;

        logger.LogDebug(
            "Saving {ChangedEntityCount} changed entity instance(s) through EF Core",
            changedEntities);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("EF Core saved {AffectedRows} database row(s)", result);
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        logger.LogError(eventData.Exception, "EF Core failed while saving database changes");
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
}
