namespace Kootam.Extensions.MessageBroker.Abstractions.Contracts;

public interface IMessageBrokerConnection
{
    bool IsConnected { get; }
    Task<bool> TryConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
}