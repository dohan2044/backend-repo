using Journey_of_faith.Application.common.interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Journey_of_faith.Infrastructure.context;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(ApplicationDbContext dbContext, ILogger<UnitOfWork> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved {AffectedRows} database row(s)", affectedRows);
            return affectedRows;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to save database changes");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            _logger.LogDebug("A database transaction is already active");
            return;
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogInformation("Started database transaction {TransactionId}", _currentTransaction.TransactionId);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
                _logger.LogInformation(
                    "Committed database transaction {TransactionId}; {AffectedRows} row(s) affected",
                    _currentTransaction.TransactionId,
                    affectedRows);
            }
            else
            {
                _logger.LogDebug("Saved {AffectedRows} row(s) without an active transaction", affectedRows);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to commit database transaction");
            await RollBackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollBackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            return;
        }

        var transactionId = _currentTransaction.TransactionId;
        await _currentTransaction.RollbackAsync(cancellationToken);
        _logger.LogWarning("Rolled back database transaction {TransactionId}", transactionId);
        await DisposeTransactionAsync();
    }

    private async ValueTask DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
        }

        _currentTransaction = null;
    }

    public void Dispose() => _dbContext.Dispose();
}
