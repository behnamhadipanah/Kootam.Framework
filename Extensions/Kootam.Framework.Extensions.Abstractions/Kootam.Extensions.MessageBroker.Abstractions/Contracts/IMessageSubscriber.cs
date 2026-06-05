using Kootam.Extensions.MessageBroker.Abstractions.Models;

namespace Kootam.Extensions.MessageBroker.Abstractions.Contracts;

public interface IMessageSubscriber
{
    Task SubscribeAsync<T>(string queueName,Func<MessageContext<T>, Task> handler,SubscribeOptions? options = null,CancellationToken cancellationToken = default);
}