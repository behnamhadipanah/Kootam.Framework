namespace Kootam.MessageBroker.Abstractions.Models;

public class PublishOptions
{
    public string? Exchange { get; set; }
    public string? RoutingKey { get; set; }
    public bool Persistent { get; set; } = true;
    public int? DelayMilliseconds { get; set; }
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}