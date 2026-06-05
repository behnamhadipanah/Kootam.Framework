using System.Text.Json;
using Kootam.MessageBroker.Abstractions.Contracts;

namespace Kootam.MessageBroker.RabbitMQ.Serialization;

public sealed class JsonMessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public byte[] Serialize<T>(T message)
        => JsonSerializer.SerializeToUtf8Bytes(message, Options);

    public T? Deserialize<T>(byte[] data)
        => JsonSerializer.Deserialize<T>(data, Options);
}
