using Kootam.Extensions.Cqrs.Abstractions.Models;

namespace Kootam.Extensions.Cqrs.Abstractions.Queries;

public interface IQueryDispatcher
{
    Task<Result<TResult>> Execute<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}