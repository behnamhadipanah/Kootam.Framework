using Kootam.Caching.Abstractions;
using Kootam.Authentication.SignIn.Contracts;

namespace Kootam.Authentication.SignIn.Services;

internal sealed class SignInCache(ICacheStore store) : ISignInCache
{
    public Task<T?> GetAsync<T>(string key) where T : class => store.GetAsync<T>(key);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        => store.SetAsync(key, value, expiry);

    public async Task RemoveAsync(string key) => await store.RemoveAsync(key);
}
