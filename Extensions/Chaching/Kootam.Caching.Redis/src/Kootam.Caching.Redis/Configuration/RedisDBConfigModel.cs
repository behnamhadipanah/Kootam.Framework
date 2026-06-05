using System.ComponentModel.DataAnnotations;

namespace Kootam.Caching.Redis.Configuration;

/// <summary>
/// Configuration model for a single Redis database connection
/// </summary>
public class RedisDBConfigModel
{
    /// <summary>
    /// Unique name identifier for this Redis configuration (e.g., "Cache", "Session")
    /// </summary>
    [Required(ErrorMessage = "Redis configuration name is required")]
    public required string Name { get; set; }

    /// <summary>
    /// Redis database number (0-15 by default in Redis)
    /// </summary>
    [Range(0, 15, ErrorMessage = "Redis database number must be between 0 and 15")]
    public int DBNumber { get; set; } = 0;

    /// <summary>
    /// Redis server host address
    /// </summary>
    [Required(ErrorMessage = "Redis host is required")]
    public required string Host { get; set; }

    /// <summary>
    /// Redis server port
    /// </summary>
    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535")]
    public int Port { get; set; } = 6379;

    /// <summary>
    /// Password for Redis authentication (optional)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Enable SSL/TLS connection
    /// </summary>
    public bool Ssl { get; set; } = false;

    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    [Range(1000, 60000, ErrorMessage = "Connect timeout must be between 1000ms and 60000ms")]
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    /// Sync operation timeout in milliseconds
    /// </summary>
    [Range(1000, 60000, ErrorMessage = "Sync timeout must be between 1000ms and 60000ms")]
    public int SyncTimeout { get; set; } = 5000;

    /// <summary>
    /// Number of connection retry attempts
    /// </summary>
    [Range(0, 10, ErrorMessage = "Connect retry must be between 0 and 10")]
    public int ConnectRetry { get; set; } = 3;

    /// <summary>
    /// Abort connection if initial connect fails
    /// </summary>
    public bool AbortOnConnectFail { get; set; } = false;

    /// <summary>
    /// Allow admin operations (FLUSHDB, etc.)
    /// </summary>
    public bool AllowAdmin { get; set; } = false;

    /// <summary>
    /// Client name for connection identification in Redis
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// Build StackExchange.Redis connection string
    /// </summary>
    public string ToConnectionString()
    {
        var parts = new List<string>
        {
            $"{Host}:{Port}",
            $"defaultDatabase={DBNumber}",
            $"connectTimeout={ConnectTimeout}",
            $"syncTimeout={SyncTimeout}",
            $"connectRetry={ConnectRetry}",
            $"abortConnect={AbortOnConnectFail.ToString().ToLower()}",
            $"allowAdmin={AllowAdmin.ToString().ToLower()}"
        };

        if (!string.IsNullOrWhiteSpace(Password))
            parts.Add($"password={Password}");

        if (Ssl)
            parts.Add("ssl=true");

        if (!string.IsNullOrWhiteSpace(ClientName))
            parts.Add($"name={ClientName}");

        return string.Join(",", parts);
    }
}