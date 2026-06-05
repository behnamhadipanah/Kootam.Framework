using Kootam.MessageBroker.Abstractions.Contracts;
using Kootam.MessageBroker.Abstractions.Exceptions;
using Kootam.MessageBroker.RabbitMQ.Helpers;
using Kootam.MessageBroker.RabbitMQ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Kootam.MessageBroker.RabbitMQ.Connection;

public class RabbitMqConnection : IMessageBrokerConnection
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnection> _logger;
    private readonly Lazy<Task<IConnection>> _connectionLazy;
    public RabbitMqConnection(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnection> logger)
    {
        _options = options.Value;
        _logger = logger;

        // ExecutionAndPublication: only one thread runs the factory;
        // all other threads wait and then share the same result.
        _connectionLazy = new Lazy<Task<IConnection>>(
            CreateConnectionInternalAsync,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    
    public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            await _connectionLazy.Value.WaitAsync(cancellationToken);
            return IsConnected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "RabbitMQ connection could not be established after {Max} retries.",
                _options.RetryCount);
            throw new BrokerConnectionException("Could not connect to RabbitMQ.", ex);
        }    }


    public bool IsConnected
    {
        get
        {
            // If Lazy hasn't been triggered yet, we're definitely not connected
            if (!_connectionLazy.IsValueCreated) return false;
            // If the Task faulted or the connection dropped, report false
            var task = _connectionLazy.Value;
            return task.IsCompletedSuccessfully && task.Result.IsOpen;
        }
    }
    
    private Task<IConnection> CreateConnectionInternalAsync()
        => RetryHelper.ExecuteAsync<IConnection>(
            async () =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = _options.HostName,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    VirtualHost = _options.VirtualHost
                };

                var connection = await factory.CreateConnectionAsync(_options.ClientProvidedName);

                _logger.LogInformation(
                    "RabbitMQ connection established to {Host}:{Port}",
                    _options.HostName, _options.Port);

                return connection;
            },
            retryCount: _options.RetryCount,
            baseDelaySeconds: _options.RetryBaseDelaySeconds,
            logger: _logger,
            operationName: "RabbitMQ.Connect");
    /// <summary>Opens a fresh channel. Caller is responsible for disposing it.</summary>
    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _connectionLazy.Value.WaitAsync(cancellationToken);
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (!_connectionLazy.IsValueCreated) return;
        var task = _connectionLazy.Value;
        if (!task.IsCompletedSuccessfully) return;

        await task.Result.CloseAsync();
        task.Result.Dispose();
    }
}