using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Kootam.Extensions.Caching.Redis.Configuration;
using Xunit;
using Kootam.Extensions.Caching.Redis.DependencyInjection;

namespace Kootam.Extensions.Caching.Redis.UnitTest;

public class RedisOptionsTests
{
    [Fact]
    public void RedisOptions_ShouldBindCorrectly_FromAppSettings()
    {
        #region Arrange

        var inMemory = new Dictionary<string, string?>
        {
            ["Redis:Configs:0:Name"] = "Default",
            ["Redis:Configs:0:Host"] = "localhost",
            ["Redis:Configs:0:Port"] = "6379",
            ["Redis:Configs:0:DBNumber"] = "0"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();

        var services = new ServiceCollection();
        services.AddRedisCaching(configuration);

        var provider = services.BuildServiceProvider();

        #endregion


        #region Act

        var options = provider.GetRequiredService<IOptions<RedisDBConfigs>>().Value;

        #endregion

        #region Assert
        Assert.NotNull(options);
        Assert.Single(options.Configs);
        Assert.Equal("localhost", options.Configs[0].Host);
        Assert.Equal(6379, options.Configs[0].Port);
        #endregion
    }
    [Fact]

    public void RedisOptions_ShouldFailValidation_WhenMissingHost()
    {
        #region Arrange

        var inMemory = new Dictionary<string, string?>
        {
            ["Redis:Configs:0:Name"] = "Default",
            // Host missing
            ["Redis:Configs:0:Port"] = "6379",
            ["Redis:Configs:0:DBNumber"] = "0"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();

        var services = new ServiceCollection();
        services.AddRedisCaching(configuration);

        var provider = services.BuildServiceProvider();

        #endregion




        #region  Act,Assert
        var ex = Assert.Throws<OptionsValidationException>(() =>
        {
            var _ = provider.GetRequiredService<IOptions<RedisDBConfigs>>().Value;
        });

        Assert.Contains("Host", ex.Message);
        #endregion

    }
    
    
    [Fact]
    public void RedisOptions_ShouldFailValidation_WhenMissingHost2()
    {
        #region Arrange
        var inMemory = new Dictionary<string, string?>
        {
            ["Redis:Configs:0:Name"] = "Default",
            // Host missing
            ["Redis:Configs:0:Port"] = "6379",
            ["Redis:Configs:0:DBNumber"] = "0"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();

        var services = new ServiceCollection();
        services.AddRedisCaching(configuration);
        #endregion

        #region Act + Assert
        var ex = Assert.Throws<OptionsValidationException>(() =>
        {
            services.BuildServiceProvider(validateScopes: true);
        });

        Assert.Contains("Host", ex.Message);
        #endregion
    }
}