using Kootam.Caching.Redis.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Kootam.Caching.Redis.Context;

/// <summary>
/// Redis connection context implementation using StackExchange.Redis
/// This is thread-safe and should be registered as Singleton
/// </summary>
public class RedisContext : IRedisContext
{
    private readonly Dictionary<string, IConnectionMultiplexer> _connections;
    private readonly Dictionary<string, int> _dbNumbers;
    private readonly string? _defaultConfigName;
    private readonly ILogger<RedisContext> _logger;
    private bool _disposed = false;
    private readonly object _disposeLock = new();

    public RedisContext(
        IOptions<RedisDBConfigs> options,
        ILogger<RedisContext> logger)
    {
        _logger = logger;
        _connections = new Dictionary<string, IConnectionMultiplexer>(StringComparer.OrdinalIgnoreCase);
        _dbNumbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var configs = options.Value;
        configs.Validate();

        foreach (var config in configs.Configs)
        {
            try
            {
                _logger.LogInformation(
                    "Connecting to Redis: {Name} at {Host}:{Port} (DB: {DBNumber})",
                    config.Name, config.Host, config.Port, config.DBNumber);

                var configOptions = ConfigurationOptions.Parse(config.ToConnectionString());


                var connection = ConnectionMultiplexer.Connect(configOptions);

                connection.ConnectionFailed += OnConnectionFailed;
                connection.ConnectionRestored += OnConnectionRestored;


                _connections[config.Name] = connection;
                _dbNumbers[config.Name] = config.DBNumber;

                _logger.LogInformation(
                    "Successfully connected to Redis: {Name}", config.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to connect to Redis: {Name} at {Host}:{Port}",
                    config.Name, config.Host, config.Port);
                throw;
            }
        }

        _defaultConfigName = configs.GetDefaultConfig()?.Name;

        if (_defaultConfigName == null)
        {
            throw new InvalidOperationException("No default Redis configuration found");
        }

        _logger.LogInformation(
            "Redis context initialized with {Count} connection(s). Default: {Default}",
            _connections.Count, _defaultConfigName);
    }

    public IDatabase GetDatabase(string? configName = null)
    {
        ThrowIfDisposed();

        var name = configName ?? _defaultConfigName
            ?? throw new InvalidOperationException("No default Redis configuration found");

        if (!_connections.TryGetValue(name, out var connection))
        {
            throw new InvalidOperationException(
                $"Redis configuration '{name}' not found. Available: {string.Join(", ", _connections.Keys)}");
        }

        if (!connection.IsConnected)
        {
            _logger.LogWarning(
                "Redis connection '{Name}' is not connected, attempting to use it anyway", name);
        }

        return connection.GetDatabase(_dbNumbers[name]);
    }

    public IServer GetServer(string? configName = null)
    {
        ThrowIfDisposed();

        var connection = GetConnection(configName);
        var endpoints = connection.GetEndPoints();

        if (endpoints.Length == 0)
        {
            throw new InvalidOperationException("No Redis endpoints available");
        }

        return connection.GetServer(endpoints[0]);
    }

    public IConnectionMultiplexer GetConnection(string? configName = null)
    {
        ThrowIfDisposed();

        var name = configName ?? _defaultConfigName
            ?? throw new InvalidOperationException("No default Redis configuration found");

        if (!_connections.TryGetValue(name, out var connection))
        {
            throw new InvalidOperationException(
                $"Redis configuration '{name}' not found. Available: {string.Join(", ", _connections.Keys)}");
        }

        return connection;
    }

    public bool ConfigExists(string configName)
    {
        return _connections.ContainsKey(configName);
    }

    public IReadOnlyList<string> GetConfigNames()
    {
        return _connections.Keys.ToList().AsReadOnly();
    }

    private void OnConnectionFailed(object? sender, ConnectionFailedEventArgs e)
    {
        _logger.LogError(
            "Redis connection failed: {EndPoint}, Type: {ConnectionType}, Failure: {FailureType}",
            e.EndPoint, e.ConnectionType, e.FailureType);
    }

    private void OnConnectionRestored(object? sender, ConnectionFailedEventArgs e)
    {
        _logger.LogInformation(
            "Redis connection restored: {EndPoint}, Type: {ConnectionType}",
            e.EndPoint, e.ConnectionType);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RedisContext));
        }
    }

    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed) return;

            _logger.LogInformation("Disposing Redis context with {Count} connection(s)", _connections.Count);

            foreach (var kvp in _connections)
            {
                try
                {
                    _logger.LogDebug("Closing Redis connection: {Name}", kvp.Key);
                    kvp.Value?.Close();
                    kvp.Value?.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing Redis connection: {Name}", kvp.Key);
                }
            }

            _connections.Clear();
            _dbNumbers.Clear();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}