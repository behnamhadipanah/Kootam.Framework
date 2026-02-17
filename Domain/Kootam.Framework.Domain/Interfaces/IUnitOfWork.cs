using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kootam.Framework.Domain.Interfaces;

public interface IUnitOfWork
{
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    bool CommitTransaction();
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);

    void Rollback();
}