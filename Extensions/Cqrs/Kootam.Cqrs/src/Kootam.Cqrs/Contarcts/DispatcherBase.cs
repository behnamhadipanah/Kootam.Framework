using Kootam.Cqrs.Abstractions.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Kootam.Cqrs.Contarcts;

public abstract class DispatcherBase(IServiceProvider provider, ILogger logger)
{
    protected async Task<object> InvokePipeline(
             object handler,
             MethodInfo handleMethod,
             object request,
             CancellationToken cancellationToken)
    {


        var requestType = request.GetType();
        var responseType = handleMethod.ReturnType.GetGenericArguments()[0];

        var behaviorType = typeof(IPipelineBehavior<,>)
            .MakeGenericType(requestType, responseType);

        var behaviors = provider.GetServices(behaviorType).Reverse().ToList();

        RequestHandlerDelegate<object> handlerDelegate = async () =>
        {
            var task = (Task)handleMethod.Invoke(handler, new[] { request, cancellationToken })!;
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task)!;
        };

        foreach (var behavior in behaviors)
        {
            var currentBehavior = behavior;
            var next = handlerDelegate;

            handlerDelegate = async () =>
            {

                logger.LogInformation("Executing behavior {BehaviorType}", currentBehavior.GetType().Name);

                var handleBehaviorMethod = currentBehavior.GetType()
                    .GetMethod("Handle", new[] { requestType, typeof(RequestHandlerDelegate<>)
                         .MakeGenericType(responseType), typeof(CancellationToken) });

                var delegateType = typeof(RequestHandlerDelegate<>).MakeGenericType(responseType);

                var wrapperMethod = typeof(DispatcherBase)
                    .GetMethod(nameof(WrapDelegate), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(responseType);

                var behaviorDelegate = wrapperMethod.Invoke(null, new object[] { next });

                var task = (Task)handleBehaviorMethod!.Invoke(currentBehavior, new[] { request, behaviorDelegate, cancellationToken })!;
                await task.ConfigureAwait(false);

                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty!.GetValue(task)!;
            };

        }

        return await handlerDelegate();
    }

    private static RequestHandlerDelegate<TResult> WrapDelegate<TResult>(RequestHandlerDelegate<object> next)
    {
        return async () =>
        {
            var result = await next();
            return (TResult)result;
        };
    }
}
