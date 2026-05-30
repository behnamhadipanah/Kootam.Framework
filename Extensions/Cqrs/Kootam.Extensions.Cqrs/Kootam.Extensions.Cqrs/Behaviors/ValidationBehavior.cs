using FluentValidation;
using Kootam.Extensions.Cqrs.Abstractions.Behaviors;
using Microsoft.Extensions.Logging;

namespace Kootam.Extensions.Cqrs.Behaviors;

public class ValidationBehavior<TRequest, TResult>
    (IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResult>> logger) 
    : IPipelineBehavior<TRequest, TResult>
{
    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {

        if (!validators.Any())
            return await next();

        logger.LogDebug("Validating {RequestType} with data {@Request}", typeof(TRequest).Name, request);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(x => x is not null).ToList();
        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();

    }
}
