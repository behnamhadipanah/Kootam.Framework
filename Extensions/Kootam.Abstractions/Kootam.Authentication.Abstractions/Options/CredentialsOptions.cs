namespace Kootam.Authentication.Abstractions.Options;

public sealed class CredentialsOptions
{
    public required string AccessToken { get; init; }

    public string? RefreshToken { get; init; }

    public DateTimeOffset AccessTokenExpires { get; init; }

    public DateTimeOffset? RefreshTokenExpires { get; init; }
}