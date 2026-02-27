using Kootam.Framework.Caching.Redis.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Kootam.Framework.Caching.Redis.HealthChecks;

/// <summary>
/// Health check for Redis connectivity
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IRedisContext _context;
    private readonly ILogger<RedisHealthCheck> _logger;

    public RedisHealthCheck(
        IRedisContext context,
        ILogger<RedisHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var configNames = _context.GetConfigNames();
            var results = new Dictionary<string, object>();
            var allHealthy = true;
            var unhealthyConfigs = new List<string>();

            foreach (var configName in configNames)
            {
                try
                {
                    var db = _context.GetDatabase(configName);
                    var pingTime = await db.PingAsync();

                    results[$"{configName}_ping_ms"] = pingTime.TotalMilliseconds;
                    results[$"{configName}_status"] = "healthy";

                    _logger.LogDebug(
                        "Redis health check passed for {ConfigName}: {PingMs}ms",
                        configName, pingTime.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    allHealthy = false;
                    unhealthyConfigs.Add(configName);
                    results[$"{configName}_status"] = "unhealthy";
                    results[$"{configName}_error"] = ex.Message;

                    _logger.LogError(ex,
                        "Redis health check failed for {ConfigName}",
                        configName);
                }
            }

            if (allHealthy)
            {
                return HealthCheckResult.Healthy(
                    $"All Redis connections ({configNames.Count}) are healthy",
                    results);
            }
            else
            {
                var message = $"Redis health check failed for: {string.Join(", ", unhealthyConfigs)}";

                // If all configs are unhealthy, return Unhealthy
                // If only some are unhealthy, return Degraded
                var status = unhealthyConfigs.Count == configNames.Count
                    ? HealthStatus.Unhealthy
                    : HealthStatus.Degraded;

                return new HealthCheckResult(
                    status,
                    message,
                    data: results);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis health check failed with exception");

            return HealthCheckResult.Unhealthy(
                "Redis health check failed",
                ex,
                new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["type"] = ex.GetType().Name
                });
        }
    }
}