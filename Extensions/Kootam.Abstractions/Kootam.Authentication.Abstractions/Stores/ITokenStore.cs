using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Stores;

public interface ITokenStore
{
    Task StoreAsync(RefreshToken token);

    Task<RefreshToken?> GetAsync(string token);

    Task RevokeAsync(string token);
}
