using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.common.interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollBackTransactionAsync(CancellationToken cancellationToken = default);
}
