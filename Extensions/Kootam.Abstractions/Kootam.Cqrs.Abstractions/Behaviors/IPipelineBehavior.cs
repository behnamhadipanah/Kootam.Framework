namespace Kootam.Cqrs.Abstractions.Behaviors;


public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

public interface IPipelineBehavior<TRequest,TResult>
{
    Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default);
}
