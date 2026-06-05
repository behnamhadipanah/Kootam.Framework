using System.Diagnostics;
using System.Reflection;
using Kootam.Cqrs.Contarcts;
using Kootam.Cqrs.Abstractions.Behaviors;
using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kootam.Cqrs.Commands;

public class RequestDispatcher(
    IServiceProvider provider,
    ILogger<RequestDispatcher> logger) : DispatcherBase(provider,logger),IRequestDispatcher
{
    private readonly Stopwatch stopwatch = new();

    public async Task<Result<TResult>> Send<TResult>(IRequest<TResult> command,
        CancellationToken cancellationToken = new CancellationToken())
    {

        stopwatch.Restart();
        try
        {
            var commandType = command.GetType();
            
            logger.LogDebug("Handling command {CommandType} with data {@Command} at {StartTime}",commandType, command, DateTime.UtcNow);
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(TResult));
            var handler = provider.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod("Handle");


            var response = await InvokePipeline(handler, handleMethod, command, cancellationToken);
            return (Result<TResult>)response;


        }
        catch (Exception ex)
        {

            logger.LogError(ex,
             "Exception occurred while handling command {CommandType} at {ErrorTime}. Command data: {@Command}",
             command.GetType(), DateTime.UtcNow, command);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Command {CommandType} executed in {Elapsed} ms",
                command.GetType(), stopwatch.ElapsedMilliseconds);
        }

  
    

    }

    public async Task<Result> Send(IRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        stopwatch.Restart();
        try
        {
            logger.LogDebug("Handling command {CommandType} with data {@Command} at {StartTime}", command.GetType(),
                command, DateTime.UtcNow);
            var requestType = command.GetType();
           var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

            var handler=provider.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod("Handle");

            var response=await InvokePipeline(handler,handleMethod,command, cancellationToken);

            return (Result)response;

        }
        catch (Exception ex)
        {
            logger.LogDebug(
                "Exception occurred while handling command {CommandType} at {ErrorTime}. Command data: {@Command}",
                command.GetType(), DateTime.Now, command);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Command {CommandType} executed in {Elapsed} ms", command.GetType(),
                stopwatch.ElapsedMilliseconds);
        }
    }

}