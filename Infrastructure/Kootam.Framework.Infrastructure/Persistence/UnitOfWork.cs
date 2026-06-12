using Kootam.Framework.Domain.Contracts;
using Kootam.Framework.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kootam.Framework.Infrastructure.Persistence;

public class UnitOfWork<TDbContext>(BaseCommandDbContext<TDbContext> context) : IUnitOfWork, IDisposable
    where TDbContext:DbContext
{
    private IDbContextTransaction? _transaction;

    public IDbContextTransaction BeginTransaction()
    {
        _transaction = context.Database.BeginTransaction();
        return _transaction;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public bool CommitTransaction()
    {
        int result = context.SaveChanges();
        _transaction?.Commit();
        _transaction = null;
        return result > 0;
    }

    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        int result = await context.SaveChangesAsync(cancellationToken);
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        _transaction = null;
        return result > 0;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction = null;

    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await action(); //per business logic
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
