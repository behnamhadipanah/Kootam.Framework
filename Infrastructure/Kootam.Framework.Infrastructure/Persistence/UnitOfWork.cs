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
        return result > 0;
    }

    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        int result = await context.SaveChangesAsync(cancellationToken);
        await _transaction?.CommitAsync(cancellationToken);
        return result > 0;
    }

    public void Dispose()
    {
        _transaction.Dispose();
        context.Dispose();
    }

    public void Rollback()
    {
        _transaction?.Rollback();

    }
}
