namespace Kootam.MessageBroker.Abstractions.Models;

public class PublishOptions
{
    public string? Exchange { get; set; }
    public string RoutingKey { get; set; } = string.Empty;
    public bool Persistent { get; set; } = true;
    public TimeSpan? Expiration { get; set; }
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}