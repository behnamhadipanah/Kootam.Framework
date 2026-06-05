namespace Kootam.Extensions.MessageBroker.Abstractions.Models;

public class SubscribeOptions
{
    public string? Exchange { get; set; }
    public string? RoutingKey { get; set; }
    public bool AutoAck { get; set; } = false;
    public ushort PrefetchCount { get; set; } = 1;
    public bool Durable { get; set; } = true;
}