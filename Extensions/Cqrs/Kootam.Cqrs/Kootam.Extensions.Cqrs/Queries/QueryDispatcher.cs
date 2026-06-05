using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Abstractions.Queries;
using Kootam.Extensions.Cqrs.Contarcts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Kootam.Extensions.Cqrs.Queries;

public class QueryDispatcher(IServiceProvider provider, ILogger<QueryDispatcher> logger) : DispatcherBase(provider,logger), IQueryDispatcher
{
    
    private readonly Stopwatch stopwatch = new();

    public async Task<Result<TResult>> Execute<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        stopwatch.Restart();
        try
        {
            var queryType = query.GetType();
            logger.LogDebug("Executing Query {QueryType} with data {@Query}  at {StartTime}", queryType, query,DateTime.UtcNow);
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
            var handler = provider.GetRequiredService(handlerType);
            var handleMethod = handler.GetType().GetMethod("Handle");
            
            var response=await InvokePipeline(handler,handleMethod,query, cancellationToken);

            return (Result<TResult>)response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    
    }
    
    
    
     
}