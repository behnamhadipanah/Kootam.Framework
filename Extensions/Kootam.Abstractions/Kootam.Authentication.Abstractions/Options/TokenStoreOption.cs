
using Kootam.Authentication.Abstractions.Models;

namespace Kootam.Authentication.Abstractions.Options;

public sealed class TokenStoreOption
{
    public required string AccessToken { get; init; }

    public RefreshToken? RefreshToken { get; init; }

    public DateTimeOffset AccessTokenExpires { get; init; }

    public DateTimeOffset? RefreshTokenExpires { get; init; }
}