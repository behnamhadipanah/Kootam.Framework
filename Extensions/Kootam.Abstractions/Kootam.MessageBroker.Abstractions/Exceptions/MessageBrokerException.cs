namespace Kootam.Extensions.MessageBroker.Abstractions.Exceptions;

public class MessageBrokerException : Exception
{
    public MessageBrokerException(string message) : base(message) { }
    public MessageBrokerException(string message, Exception inner) : base(message, inner) { }
}