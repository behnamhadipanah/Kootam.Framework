using Kootam.Cqrs.Exceptions;
using Kootam.Cqrs.Abstractions.Behaviors;
using Kootam.Cqrs.Abstractions.Enums;
using Kootam.Cqrs.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace Kootam.Cqrs.Commands;

public class DomainExceptionBehavior<TRequest, TResult>
    : IPipelineBehavior<TRequest, TResult>
    where TResult : Result
{
    private readonly ILogger<DomainExceptionBehavior<TRequest, TResult>> _logger;

    public DomainExceptionBehavior(
        ILogger<DomainExceptionBehavior<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Handle(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainStateException ex)
        {
            _logger.LogError(
                ex,
                "Domain exception while processing {RequestType}",
                typeof(TRequest).Name);

            var result = Result.Failure(ResultStatus.ValidationError,ex.Message);

            return (TResult)(object)result;
        }
    }
}
