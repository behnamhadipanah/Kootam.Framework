using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Abstractions.Queries;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken = default);
}