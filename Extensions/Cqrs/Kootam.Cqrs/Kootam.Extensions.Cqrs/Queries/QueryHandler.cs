using Kootam.Extensions.Cqrs.Abstractions.Enums;
using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Abstractions.Queries;

namespace Kootam.Extensions.Cqrs.Queries;


public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    public abstract Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken);

    protected Result<TResult> Ok(TResult data, string? message = null)
        => Result<TResult>.Success(data, message);

    protected Result<TResult> Fail(ResultStatus status, params string[] messages)
        => Result<TResult>.Failure(status, messages);
}