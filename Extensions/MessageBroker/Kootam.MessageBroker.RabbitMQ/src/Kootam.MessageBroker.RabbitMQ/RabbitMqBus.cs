using Kootam.MessageBroker.Abstractions.Contracts;
using Kootam.MessageBroker.Abstractions.Models;
using Kootam.MessageBroker.RabbitMQ.Publisher;
using Kootam.MessageBroker.RabbitMQ.Subscriber;

namespace Kootam.MessageBroker.RabbitMQ;
/// <summary>
/// Composite implementation of IMessageBus.
/// Use this when a service needs to both publish and subscribe.
/// </summary>
public sealed class RabbitMqBus : IMessageBus
{
    private readonly RabbitMqPublisher _publisher;
    private readonly RabbitMqSubscriber _subscriber;

    public RabbitMqBus(RabbitMqPublisher publisher, RabbitMqSubscriber subscriber)
    {
        _publisher = publisher;
        _subscriber = subscriber;
    }

    public Task PublishAsync<T>(T message, string queueName, PublishOptions? options = null, CancellationToken cancellationToken = default)
        => _publisher.PublishAsync(message, queueName, options, cancellationToken);

    public Task PublishAsync<T>(MessageEnvelope<T> envelope, string queueName, PublishOptions? options = null, CancellationToken cancellationToken = default)
        => _publisher.PublishAsync(envelope, queueName, options, cancellationToken);

    public Task SubscribeAsync<T>(string queueName, Func<MessageContext<T>, Task> handler, SubscribeOptions? options = null, CancellationToken cancellationToken = default)
        => _subscriber.SubscribeAsync(queueName, handler, options, cancellationToken);
}
