using Microsoft.Extensions.Logging;

namespace Kootam.MessageBroker.RabbitMQ.Helpers;

internal static class RetryHelper
{
    /// <summary>
    /// Executes <paramref name="action"/> with exponential backoff retry.
    /// Throws the last exception if all attempts fail.
    /// </summary>
    internal static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        int retryCount,
        int baseDelaySeconds,
        ILogger logger,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        for (int attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (attempt < retryCount && !cancellationToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(baseDelaySeconds, attempt));

                logger.LogWarning(ex,
                    "{Operation} attempt {Attempt}/{Max} failed. Retrying in {Delay}s.",
                    operationName, attempt, retryCount, delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
            }
        }

        // Last attempt — let the exception propagate naturally
        return await action();
    }

    /// <summary>Overload for void-like operations (Task without result).</summary>
    internal static Task ExecuteAsync(
        Func<Task> action,
        int retryCount,
        int baseDelaySeconds,
        ILogger logger,
        string operationName,
        CancellationToken cancellationToken = default)
        => ExecuteAsync<bool>(
            async () => { await action(); return true; },
            retryCount, baseDelaySeconds, logger, operationName, cancellationToken);
}