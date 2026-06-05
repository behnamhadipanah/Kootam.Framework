using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Abstractions.Queries;

public interface IQueryDispatcher
{
    Task<Result<TResult>> Execute<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}