namespace Kootam.MessageBroker.RabbitMQ.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ClientProvidedName { get; set; } = "Kootam.MessageBroker";

    /// <summary>Max retry attempts when connecting or publishing.</summary>
    public int RetryCount { get; set; } = 5;

    /// <summary>Base delay (seconds) for exponential backoff.</summary>
    public int RetryBaseDelaySeconds { get; set; } = 2;
}