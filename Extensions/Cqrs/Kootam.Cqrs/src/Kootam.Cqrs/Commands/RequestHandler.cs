using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Enums;
using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Commands;

public abstract class RequestHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : IRequest<TResult>
{

    public abstract Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
    protected virtual Result<TResult> Ok(TResult result, string? message = null)
    => Result<TResult>.Success(result, message);

    protected virtual Result<TResult> Fail(ResultStatus status, params string[] messages)
    => Result<TResult>.Failure(status, messages);
}

public abstract class RequestHandler<TCommand> : IRequestHandler<TCommand> where TCommand : IRequest
{

    public abstract Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
    protected virtual Result Ok()
    => Result.Success();

    protected virtual Result Fail(ResultStatus status, params string[] messages)
    => Result.Failure(status, messages);
}
