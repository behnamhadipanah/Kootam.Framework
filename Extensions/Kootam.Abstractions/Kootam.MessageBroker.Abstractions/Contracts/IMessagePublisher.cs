using Kootam.MessageBroker.Abstractions.Models;

namespace Kootam.MessageBroker.Abstractions.Contracts;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message,string queueName,PublishOptions? options = null,CancellationToken cancellationToken = default);
    Task PublishAsync<T>(MessageEnvelope<T> envelope,string queueName,PublishOptions? options = null,CancellationToken cancellationToken = default);
}