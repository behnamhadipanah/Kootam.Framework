namespace Kootam.MessageBroker.Abstractions.Contracts;

public interface IMessageBrokerConnection : IAsyncDisposable
{
    bool IsConnected { get; }
    Task<bool> TryConnectAsync(CancellationToken cancellationToken = default);
}
