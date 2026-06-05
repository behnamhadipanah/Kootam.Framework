namespace Kootam.MessageBroker.Abstractions.Models;

public class MessageContext<T>
{
    public T Message { get; init; } = default!;
    public MessageMetadata Metadata { get; init; } = new();
    public CancellationToken CancellationToken { get; init; }
}