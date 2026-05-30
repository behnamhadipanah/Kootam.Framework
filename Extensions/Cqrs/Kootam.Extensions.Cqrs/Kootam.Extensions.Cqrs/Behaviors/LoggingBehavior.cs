using System.Diagnostics;
using Kootam.Extensions.Cqrs.Abstractions.Behaviors;
using Microsoft.Extensions.Logging;

namespace Kootam.Extensions.Cqrs.Behaviors;

public class LoggingBehavior<TRequest, TResult>(ILogger<LoggingBehavior<TRequest, TResult>> logger)
    : IPipelineBehavior<TRequest, TResult>
{


    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogDebug("Handling {RequestType} with data {@Request}", typeof(TRequest).Name, request);

        var response = await next();

        stopwatch.Stop();

        logger.LogInformation("{RequestType} executed in {Elapsed} ms", typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
