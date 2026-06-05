namespace Kootam.MessageBroker.Abstractions.Models;

public class MessageContext<T>
{
    public required T Message { get; init; }
    public MessageMetadata Metadata { get; init; } = new();
    public CancellationToken CancellationToken { get; init; }
}