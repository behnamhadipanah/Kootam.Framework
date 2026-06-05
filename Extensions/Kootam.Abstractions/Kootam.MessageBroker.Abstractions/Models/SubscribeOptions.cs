namespace Kootam.MessageBroker.Abstractions.Models;

public class SubscribeOptions
{
    public string? Exchange { get; set; }
    public string ExchangeType { get; set; } = "direct";
    public string RoutingKey { get; set; } = string.Empty;
    public bool AutoAck { get; set; } = false;
    public ushort PrefetchCount { get; set; } = 1;
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
}