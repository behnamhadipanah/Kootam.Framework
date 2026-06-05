using Kootam.Cqrs.Abstractions.Behaviors;
using Kootam.Cqrs.Abstractions.Commands;
using Microsoft.Extensions.Logging;

namespace Kootam.Cqrs.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResult> : 
    IPipelineBehavior<TRequest, TResult> where TRequest : IRequest<TResult>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }
    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(e, $"Application request : Unhandled exception for request {requestName} {request}");
            throw;
        }
    }
}

