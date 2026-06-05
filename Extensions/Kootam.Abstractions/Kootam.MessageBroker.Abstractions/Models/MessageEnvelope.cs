namespace Kootam.MessageBroker.Abstractions.Models;

public class MessageEnvelope<T>
{
    public T Payload { get; init; } = default!;
    public MessageMetadata Metadata { get; init; } = new();
}