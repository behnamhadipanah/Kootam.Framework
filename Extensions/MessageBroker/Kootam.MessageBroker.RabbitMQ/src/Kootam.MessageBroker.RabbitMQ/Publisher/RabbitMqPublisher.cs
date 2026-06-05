using Kootam.MessageBroker.Abstractions.Contracts;
using Kootam.MessageBroker.Abstractions.Exceptions;
using Kootam.MessageBroker.Abstractions.Models;
using Kootam.MessageBroker.RabbitMQ.Connection;
using Kootam.MessageBroker.RabbitMQ.Helpers;
using Kootam.MessageBroker.RabbitMQ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Kootam.MessageBroker.RabbitMQ.Publisher;


public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly RabbitMqConnection _connection;
    private readonly IMessageSerializer _serializer;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(
        RabbitMqConnection connection,
        IMessageSerializer serializer,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _serializer = serializer;
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync<T>(
        T message,
        string queueName,
        PublishOptions? options = null,
        CancellationToken cancellationToken = default)
        => PublishAsync(new MessageEnvelope<T> { Payload = message }, queueName, options, cancellationToken);

    public async Task PublishAsync<T>(
        MessageEnvelope<T> envelope,
        string queueName,
        PublishOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await RetryHelper.ExecuteAsync(
                () => PublishInternalAsync(envelope, queueName, options, cancellationToken),
                retryCount: _options.RetryCount,
                baseDelaySeconds: _options.RetryBaseDelaySeconds,
                logger: _logger,
                operationName: $"RabbitMQ.Publish[{queueName}]",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is not MessagePublishException)
        {
            throw new MessagePublishException(queueName, ex);
        }
    }

    private async Task PublishInternalAsync<T>(
        MessageEnvelope<T> envelope,
        string queueName,
        PublishOptions? options,
        CancellationToken cancellationToken)
    {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var props = new BasicProperties
        {
            Persistent = options?.Persistent ?? true,
            MessageId = envelope.Metadata.MessageId,
            CorrelationId = envelope.Metadata.CorrelationId,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            ContentType = "application/json"
        };

        if (envelope.Metadata.Headers.Any())
            props.Headers = envelope.Metadata.Headers
                .ToDictionary(k => k.Key, v => (object?)v.Value);

        if (options?.Expiration.HasValue == true)
            props.Expiration = ((long)options.Expiration.Value.TotalMilliseconds).ToString();

        var body = _serializer.Serialize(envelope);

        await channel.BasicPublishAsync(
            exchange: options?.Exchange ?? string.Empty,
            routingKey: options?.RoutingKey ?? queueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogDebug(
            "Published message {MessageId} to queue '{Queue}'.",
            envelope.Metadata.MessageId, queueName);
    }
}
