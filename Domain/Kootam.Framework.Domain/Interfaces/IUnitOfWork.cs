using Microsoft.EntityFrameworkCore.Storage;

namespace Kootam.Framework.Domain.Interfaces;

public interface IUnitOfWork
{
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    bool CommitTransaction();
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);

    void Rollback();
}