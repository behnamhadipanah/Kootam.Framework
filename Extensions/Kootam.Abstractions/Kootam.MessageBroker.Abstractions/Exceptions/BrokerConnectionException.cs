namespace Kootam.MessageBroker.Abstractions.Exceptions;

public sealed class BrokerConnectionException : MessageBrokerException
{
    public BrokerConnectionException(string message) : base(message) { }
    public BrokerConnectionException(string message, Exception inner) : base(message, inner) { }
}