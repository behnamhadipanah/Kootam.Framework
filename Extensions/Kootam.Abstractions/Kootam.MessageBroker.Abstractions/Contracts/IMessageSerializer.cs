namespace Kootam.MessageBroker.Abstractions.Contracts;

public interface IMessageSerializer
{
    byte[] Serialize<T>(T message);
    T? Deserialize<T>(byte[] data);
}