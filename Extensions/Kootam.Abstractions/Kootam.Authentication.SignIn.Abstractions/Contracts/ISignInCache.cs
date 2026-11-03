namespace Kootam.Authentication.SignIn.Contracts;

public interface ISignInCache
{
    Task<T?> GetAsync<T>(string key) where T : class;

    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;

    Task RemoveAsync(string key);
}
