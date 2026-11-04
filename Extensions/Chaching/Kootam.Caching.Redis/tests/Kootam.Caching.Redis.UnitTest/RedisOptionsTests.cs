using Kootam.Caching.Abstractions;
using Kootam.Caching.Redis.Adapter;
using Kootam.Caching.Redis.Configuration;
using Kootam.Caching.Redis.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

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

    [Fact]
    public void AddRedisCaching_registers_one_adapter_as_store_and_adapter()
    {
        var services = new ServiceCollection();
        services.AddRedisCaching(host: "localhost");

        var concrete = Assert.Single(services, static descriptor => descriptor.ServiceType == typeof(RedisCacheAdapter));
        var store = Assert.Single(services, static descriptor => descriptor.ServiceType == typeof(ICacheStore));
        var adapter = Assert.Single(services, static descriptor => descriptor.ServiceType == typeof(ICacheAdapter));
        Assert.Equal(ServiceLifetime.Singleton, concrete.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, store.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, adapter.Lifetime);

        var sentinel = new RedisCacheAdapter(null!, null!);
        var provider = new FixedProvider(sentinel);
        Assert.Same(sentinel, store.ImplementationFactory!(provider));
        Assert.Same(sentinel, adapter.ImplementationFactory!(provider));
    }

    private sealed class FixedProvider(object service) : IServiceProvider
    {
        public object? GetService(Type serviceType) => service;
    }
}