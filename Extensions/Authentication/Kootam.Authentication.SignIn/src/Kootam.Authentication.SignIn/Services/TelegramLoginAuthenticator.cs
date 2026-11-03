using System.Security.Cryptography;
using System.Text;
using Kootam.Authentication.SignIn.Models;

namespace Kootam.Authentication.SignIn.Services;

public static class TelegramLoginAuthenticator
{
    public static readonly TimeSpan MaxAge = TimeSpan.FromHours(24);

    public static bool TryValidate(TelegramLoginData data, string botToken, DateTimeOffset utcNow)
    {
        if (data.Id <= 0 || data.AuthDate <= 0 || string.IsNullOrWhiteSpace(data.Hash) || string.IsNullOrWhiteSpace(botToken))
            return false;

        var authenticatedAt = DateTimeOffset.FromUnixTimeSeconds(data.AuthDate);
        if (authenticatedAt > utcNow.AddMinutes(2) || utcNow - authenticatedAt > MaxAge)
            return false;

        var expected = ComputeHash(data, botToken);
        var provided = Encoding.UTF8.GetBytes(data.Hash.Trim().ToLowerInvariant());
        var actual = Encoding.UTF8.GetBytes(expected);
        return provided.Length == actual.Length
               && CryptographicOperations.FixedTimeEquals(provided, actual);
    }

    public static string ComputeHash(TelegramLoginData data, string botToken)
    {
        var pairs = new List<string>();
        Add(pairs, "auth_date", data.AuthDate.ToString());
        Add(pairs, "first_name", data.FirstName);
        Add(pairs, "id", data.Id.ToString());
        Add(pairs, "last_name", data.LastName);
        Add(pairs, "photo_url", data.PhotoUrl);
        Add(pairs, "username", data.Username);
        pairs.Sort(StringComparer.Ordinal);

        var secret = SHA256.HashData(Encoding.UTF8.GetBytes(botToken));
        var hash = HMACSHA256.HashData(secret, Encoding.UTF8.GetBytes(string.Join('\n', pairs)));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static void Add(List<string> pairs, string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            pairs.Add(key + "=" + value);
    }
}
