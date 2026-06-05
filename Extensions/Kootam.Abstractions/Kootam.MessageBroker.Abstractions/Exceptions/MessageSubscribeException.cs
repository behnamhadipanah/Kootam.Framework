namespace Kootam.MessageBroker.Abstractions.Exceptions;

public class MessageSubscribeException : MessageBrokerException
{
    public string QueueName { get; }
    public MessageSubscribeException(string queueName, Exception inner)
        : base($"Failed to subscribe to queue '{queueName}'", inner)
        => QueueName = queueName;
}