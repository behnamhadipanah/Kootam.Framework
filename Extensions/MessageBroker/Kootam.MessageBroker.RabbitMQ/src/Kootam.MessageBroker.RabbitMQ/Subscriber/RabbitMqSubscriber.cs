using Kootam.MessageBroker.Abstractions.Contracts;
using Kootam.MessageBroker.Abstractions.Exceptions;
using Kootam.MessageBroker.Abstractions.Models;
using Kootam.MessageBroker.RabbitMQ.Connection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Kootam.MessageBroker.RabbitMQ.Subscriber;
public sealed class RabbitMqSubscriber : IMessageSubscriber
{
    private readonly RabbitMqConnection _connection;
    private readonly IMessageSerializer _serializer;
    private readonly ILogger<RabbitMqSubscriber> _logger;

    public RabbitMqSubscriber(
        RabbitMqConnection connection,
        IMessageSerializer serializer,
        ILogger<RabbitMqSubscriber> logger)
    {
        _connection = connection;
        _serializer = serializer;
        _logger = logger;
    }

    public async Task SubscribeAsync<T>(
        string queueName,
        Func<MessageContext<T>, Task> handler,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var opts = options ?? new SubscribeOptions();

        try
        {
            // Channel is kept open for the lifetime of the subscription
            var channel = await _connection.CreateChannelAsync(cancellationToken);

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: opts.PrefetchCount,
                global: false,
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: opts.Durable,
                exclusive: opts.Exclusive,
                autoDelete: opts.AutoDelete,
                cancellationToken: cancellationToken);

            // Bind to exchange if provided
            if (!string.IsNullOrWhiteSpace(opts.Exchange))
            {
                await channel.ExchangeDeclareAsync(
                    exchange: opts.Exchange,
                    type: opts.ExchangeType,
                    durable: true,
                    cancellationToken: cancellationToken);

                await channel.QueueBindAsync(
                    queue: queueName,
                    exchange: opts.Exchange,
                    routingKey: opts.RoutingKey,
                    cancellationToken: cancellationToken);
            }

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var messageId = ea.BasicProperties.MessageId ?? "unknown";

                try
                {
                    var envelope = _serializer.Deserialize<MessageEnvelope<T>>(ea.Body.ToArray());

                    if (envelope is null)
                    {
                        _logger.LogWarning("Received null envelope on queue '{Queue}'. MessageId={Id}", queueName, messageId);
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                        return;
                    }

                    var context = new MessageContext<T>
                    {
                        Message = envelope.Payload,
                        Metadata = envelope.Metadata,
                        CancellationToken = cancellationToken
                    };

                    await handler(context);

                    if (!opts.AutoAck)
                        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                    _logger.LogDebug("Processed message {Id} from queue '{Queue}'.", messageId, queueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message {Id} from queue '{Queue}'.", messageId, queueName);

                    if (!opts.AutoAck)
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: opts.AutoAck,
                consumer: consumer,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Subscribed to queue '{Queue}'.", queueName);
        }
        catch (Exception ex)
        {
            throw new MessageSubscribeException(queueName, ex);
        }
    }
}
