using Microsoft.EntityFrameworkCore.Storage;

namespace Kootam.Framework.Domain.Contracts;

public interface IUnitOfWork
{
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    bool CommitTransaction();
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);

    void Rollback();

    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);

}