namespace Kootam.MessageBroker.Abstractions.Models;

public sealed class MessageEnvelope<T>
{
    public required T Payload { get; init; }
    public MessageMetadata Metadata { get; init; } = new();
}
