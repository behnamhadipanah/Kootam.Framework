using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Abstractions.Commands;

public interface IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    Task<Result<TResult>> Handle(TRequest command, CancellationToken cancellationToken = default);
}


public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    Task<Result> Handle(TRequest command, CancellationToken cancellationToken = default);
}
