using Kootam.Cqrs.Abstractions.Enums;
using Kootam.Cqrs.Abstractions.Models;
using Kootam.Cqrs.Abstractions.Queries;

namespace Kootam.Cqrs.Queries;


public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    public abstract Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken);

    protected Result<TResult> Ok(TResult data, string? message = null)
        => Result<TResult>.Success(data, message);

    protected Result<TResult> Fail(ResultStatus status, params string[] messages)
        => Result<TResult>.Failure(status, messages);
}