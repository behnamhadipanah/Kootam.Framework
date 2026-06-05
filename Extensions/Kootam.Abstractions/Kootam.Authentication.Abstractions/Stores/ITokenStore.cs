using Kootam.Extensions.Authentication.Abstractions.Models;

namespace Kootam.Extensions.Authentication.Abstractions.Stores;

public interface ITokenStore
{
    Task StoreAsync(RefreshToken token);

    Task<RefreshToken?> GetAsync(string token);

    Task RevokeAsync(string token);
}
