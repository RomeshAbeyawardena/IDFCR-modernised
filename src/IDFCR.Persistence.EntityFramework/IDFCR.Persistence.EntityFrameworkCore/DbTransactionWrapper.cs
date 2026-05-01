using IDFCR.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace IDFCR.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a wrapper around an Entity Framework Core database transaction, implementing the IDbTransaction interface to provide a consistent transactional API for the application. This class encapsulates the underlying IDbContextTransaction and delegates commit, rollback, and disposal operations to it, allowing the application to manage transactions in a way that is decoupled from the specific implementation of the data access layer.
/// </summary>
/// <param name="transaction">The underlying Entity Framework Core database transaction to be wrapped.</param>
internal class DbTransactionWrapper(IDbContextTransaction transaction) : IDbTransaction
{
    private readonly IDbContextTransaction _transaction = transaction;
    public void Commit()
    {
        _transaction.Commit();
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return _transaction.CommitAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _transaction.DisposeAsync();
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        return _transaction.RollbackAsync(cancellationToken);
    }
}
