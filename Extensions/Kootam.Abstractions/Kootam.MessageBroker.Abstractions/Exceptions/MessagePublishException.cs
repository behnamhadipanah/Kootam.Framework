namespace Kootam.Extensions.MessageBroker.Abstractions.Exceptions;

public class MessagePublishException : MessageBrokerException
{
    public string QueueName { get; }
    public MessagePublishException(string queueName, Exception inner)
        : base($"Failed to publish message to queue '{queueName}'", inner)
        => QueueName = queueName;
}