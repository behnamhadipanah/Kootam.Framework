using Kootam.Caching.Abstractions;
using Kootam.Caching.InMemory.Adapter;
using Kootam.Caching.InMemory.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kootam.Caching.InMemory.UnitTest;

public class InMemoryCacheAdapterTests
{
    [Fact]
    public async Task Expired_entry_is_absent_on_the_next_read()
    {
        var clock = new FixedTimeProvider(new DateTimeOffset(2026, 11, 4, 8, 0, 0, TimeSpan.Zero));
        var cache = new InMemoryCacheAdapter(clock);
        await cache.SetAsync("code", new Payload("123456"), TimeSpan.FromMinutes(10));

        clock.Move(TimeSpan.FromMinutes(11));

        Assert.Null(await cache.GetAsync<Payload>("code"));
        Assert.False(await cache.ExistsAsync("code"));
    }

    [Fact]
    public async Task Entry_without_expiry_remains()
    {
        var cache = new InMemoryCacheAdapter();
        await cache.SetAsync("code", new Payload("123456"));

        var stored = await cache.GetAsync<Payload>("code");

        Assert.NotNull(stored);
        Assert.Equal("123456", stored.Value);
        Assert.True(await cache.RemoveAsync("code"));
        Assert.Null(await cache.GetAsync<Payload>("code"));
    }

    [Fact]
    public void AddInMemoryCaching_resolves_the_store()
    {
        var services = new ServiceCollection();
        services.AddInMemoryCaching();

        using var provider = services.BuildServiceProvider();

        Assert.IsType<InMemoryCacheAdapter>(provider.GetRequiredService<ICacheStore>());
    }

    private sealed record Payload(string Value);

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private DateTimeOffset utcNow = utcNow;

        public void Move(TimeSpan by) => utcNow = utcNow.Add(by);

        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
