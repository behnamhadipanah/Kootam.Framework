
using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Options;

public sealed class TokenStoreOption<TUserKey>
{
    public required string AccessToken { get; init; }

    public RefreshToken<TUserKey>? RefreshToken { get; init; }

    public DateTimeOffset AccessTokenExpires { get; init; }

    public DateTimeOffset? RefreshTokenExpires { get; init; }
}