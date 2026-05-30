using Kootam.Framework.Domain.Contracts;
using Kootam.Framework.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kootam.Framework.Infrastructure.Persistence;

public class UnitOfWork<TDbContext>(BaseCommandDbContext<TDbContext> context, IDbContextTransaction transaction) : IUnitOfWork, IDisposable
    where TDbContext:DbContext
{

    public IDbContextTransaction BeginTransaction()
    {
        transaction = context.Database.BeginTransaction();
        return transaction;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return transaction;
    }

    public bool CommitTransaction()
    {
        int result = context.SaveChanges();
        transaction?.Commit();
        return result > 0;
    }

    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        int result = await context.SaveChangesAsync(cancellationToken);
        await transaction?.CommitAsync(cancellationToken);
        return result > 0;
    }

    public void Dispose()
    {
        transaction.Dispose();
        context.Dispose();
    }

    public void Rollback()
    {
        transaction?.Rollback();

    }
}
