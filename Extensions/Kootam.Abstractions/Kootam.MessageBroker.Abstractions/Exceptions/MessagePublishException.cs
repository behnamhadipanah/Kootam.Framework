namespace Kootam.MessageBroker.Abstractions.Exceptions;

public sealed class MessagePublishException : MessageBrokerException
{
    public string QueueName { get; }

    public MessagePublishException(string queueName, Exception inner)
        : base($"Failed to publish message to queue '{queueName}'.", inner)
        => QueueName = queueName;
}