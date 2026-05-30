using System.ComponentModel.DataAnnotations;

namespace Kootam.Extensions.Caching.Redis.Configuration;
/// <summary>
/// Root configuration for Redis database connections
/// </summary>
public class RedisDBConfigs
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// List of Redis database configurations
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one Redis configuration is required")]
    public List<RedisDBConfigModel> Configs { get; set; } = new();

    /// <summary>
    /// Get configuration by name
    /// </summary>
    public RedisDBConfigModel? GetConfig(string name)
    {
        return Configs.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get default (first) configuration
    /// </summary>
    public RedisDBConfigModel? GetDefaultConfig()
    {
        return Configs.FirstOrDefault();
    }

    /// <summary>
    /// Validate configuration
    /// </summary>
    public void Validate()
    {
        if (Configs == null || !Configs.Any())
        {
            throw new InvalidOperationException("At least one Redis configuration is required");
        }

        var duplicates = Configs
            .GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Any())
        {
            throw new InvalidOperationException(
                $"Duplicate Redis configuration names found: {string.Join(", ", duplicates)}");
        }

        foreach (var config in Configs)
        {
            if (string.IsNullOrWhiteSpace(config.Name))
            {
                throw new InvalidOperationException("Redis configuration name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(config.Host))
            {
                throw new InvalidOperationException(
                    $"Host is required for Redis configuration '{config.Name}'");
            }

            if (config.Port <= 0 || config.Port > 65535)
            {
                throw new InvalidOperationException(
                    $"Invalid port {config.Port} for Redis configuration '{config.Name}'");
            }

            if (config.DBNumber < 0 || config.DBNumber > 15)
            {
                throw new InvalidOperationException(
                    $"Invalid database number {config.DBNumber} for Redis configuration '{config.Name}'. Must be 0-15.");
            }
        }
    }
}