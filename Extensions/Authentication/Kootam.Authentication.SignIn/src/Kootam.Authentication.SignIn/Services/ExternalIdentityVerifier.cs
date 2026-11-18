using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Kootam.Authentication.Abstractions.Enums;
using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Caching.Abstractions;
using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kootam.Authentication.SignIn.Services;

public sealed class ExternalIdentityVerifier(
    IOptions<SignInOptions> options,
    IHttpClientFactory httpClientFactory,
    ICacheStore cache,
    TimeProvider timeProvider,
    ILogger<ExternalIdentityVerifier> logger) : IExternalIdentityVerifier
{
    public const string HttpClientName = "KootamExternalSignIn";

    public async Task<ExternalVerificationResult> VerifyAsync(
        ExternalSignInRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var external = options.Value.External;
        if (!external.IsConfigured(request.Provider))
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Disabled);

        try
        {
            return request.Provider switch
            {
                ExternalSignInProvider.Google => await VerifyOidcAsync(
                    request.IdToken,
                    external.GoogleAudiences(),
                    "https://www.googleapis.com/oauth2/v3/certs",
                    OidcIssuerRule.Google,
                    ExternalSignInProvider.Google,
                    cancellationToken),
                ExternalSignInProvider.Apple => await VerifyOidcAsync(
                    request.IdToken,
                    external.Apple.EffectiveClientIds(),
                    "https://appleid.apple.com/auth/keys",
                    OidcIssuerRule.Apple,
                    ExternalSignInProvider.Apple,
                    cancellationToken),
                ExternalSignInProvider.Microsoft => await VerifyOidcAsync(
                    request.IdToken,
                    external.Microsoft.EffectiveClientIds(),
                    MicrosoftKeysUrl(external.Microsoft.Tenant),
                    OidcIssuerRule.Microsoft,
                    ExternalSignInProvider.Microsoft,
                    cancellationToken),
                ExternalSignInProvider.Facebook => await VerifyFacebookAsync(request.AccessToken, cancellationToken),
                ExternalSignInProvider.Telegram => VerifyTelegram(request.Telegram),
                ExternalSignInProvider.Oidc => await VerifyDiscoveredOidcAsync(request.IdToken, cancellationToken),
                _ => ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed)
            };
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or SecurityTokenException)
        {
            logger.LogWarning(exception, "External sign-in verification failed for {Provider}.", request.Provider);
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);
        }
    }

    private async Task<ExternalVerificationResult> VerifyOidcAsync(
        string? idToken,
        IEnumerable<string> clientIds,
        string jwksUrl,
        OidcIssuerRule issuerRule,
        ExternalSignInProvider provider,
        CancellationToken cancellationToken,
        string? expectedIssuer = null)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        var keys = await GetSigningKeysAsync(jwksUrl, cancellationToken);
        var document = await OidcIdTokenValidator.ValidateAsync(
            idToken,
            keys,
            clientIds,
            issuerRule,
            expectedIssuer: expectedIssuer);
        if (document is null)
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        var email = string.IsNullOrWhiteSpace(document.Email) ? null : document.Email.Trim();
        var userName = provider == ExternalSignInProvider.Oidc ? document.PreferredUserName : null;
        return ExternalVerificationResult.Success(new ExternalIdentity(
            provider,
            document.Subject,
            email,
            email is not null && document.EmailVerified,
            document.GivenName,
            document.FamilyName,
            userName));
    }

    private async Task<ExternalVerificationResult> VerifyFacebookAsync(
        string? accessToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        var facebook = options.Value.External.Facebook;
        var client = httpClientFactory.CreateClient(HttpClientName);
        var appToken = Uri.EscapeDataString($"{facebook.AppId}|{facebook.AppSecret}");
        var input = Uri.EscapeDataString(accessToken);
        var debug = await client.GetFromJsonAsync<FacebookDebugResponse>(
            $"https://graph.facebook.com/debug_token?input_token={input}&access_token={appToken}",
            cancellationToken);

        if (debug?.Data is null
            || !debug.Data.IsValid
            || !string.Equals(debug.Data.AppId, facebook.AppId, StringComparison.Ordinal))
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        var proof = ComputeAppSecretProof(accessToken, facebook.AppSecret!);
        var profile = await client.GetFromJsonAsync<FacebookProfile>(
            $"https://graph.facebook.com/me?fields=id,email,first_name,last_name&access_token={input}&appsecret_proof={Uri.EscapeDataString(proof)}",
            cancellationToken);

        if (profile is null || string.IsNullOrWhiteSpace(profile.Id))
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        var email = string.IsNullOrWhiteSpace(profile.Email) ? null : profile.Email.Trim();
        return ExternalVerificationResult.Success(new ExternalIdentity(
            ExternalSignInProvider.Facebook,
            profile.Id,
            email,
            email is not null,
            profile.FirstName,
            profile.LastName,
            null));
    }

    private ExternalVerificationResult VerifyTelegram(TelegramLoginData? data)
    {
        if (data is null || !TelegramLoginAuthenticator.TryValidate(data, options.Value.External.Telegram.BotToken!, timeProvider.GetUtcNow()))
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        return ExternalVerificationResult.Success(new ExternalIdentity(
            ExternalSignInProvider.Telegram,
            data.Id.ToString(),
            null,
            false,
            data.FirstName,
            data.LastName,
            data.Username));
    }

    private async Task<ExternalVerificationResult> VerifyDiscoveredOidcAsync(
        string? idToken,
        CancellationToken cancellationToken)
    {
        var oidc = options.Value.External.Oidc;
        var authority = oidc.Issuer();
        if (authority is null)
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Disabled);

        var discovery = await GetDiscoveryAsync(authority, cancellationToken);
        if (discovery?.Issuer is null || discovery.JwksUri is null)
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        return await VerifyOidcAsync(
            idToken,
            oidc.EffectiveClientIds(),
            discovery.JwksUri,
            OidcIssuerRule.Configured,
            ExternalSignInProvider.Oidc,
            cancellationToken,
            discovery.Issuer);
    }

    private async Task<OidcDiscoveryDocument?> GetDiscoveryAsync(string authority, CancellationToken cancellationToken)
    {
        var url = $"{authority}/.well-known/openid-configuration";
        var json = await GetCachedDocumentAsync(url, "auth:oidc:", cancellationToken, storeDownloaded: false);
        if (string.IsNullOrWhiteSpace(json))
            return null;

        OidcDiscoveryDocument? document;
        try
        {
            document = JsonSerializer.Deserialize<OidcDiscoveryDocument>(json);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "OIDC discovery document at {Url} is not valid JSON.", url);
            return null;
        }

        if (!DiscoveryAccepted(authority, document))
        {
            logger.LogWarning("OIDC discovery at {Url} does not match authority {Authority}.", url, authority);
            return null;
        }

        await StoreCachedDocumentAsync(url, "auth:oidc:", json);
        return document;
    }

    private static bool DiscoveryAccepted(string authority, OidcDiscoveryDocument? document)
    {
        if (document?.Issuer is null || string.IsNullOrWhiteSpace(document.JwksUri))
            return false;

        if (!string.Equals(NormalizeIssuer(document.Issuer), NormalizeIssuer(authority), StringComparison.Ordinal))
            return false;

        if (!Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri)
            || !Uri.TryCreate(document.JwksUri, UriKind.Absolute, out var jwksUri))
            return false;

        return string.Equals(jwksUri.Scheme, authorityUri.Scheme, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeIssuer(string value)
        => value.Trim().TrimEnd('/');

    private async Task<IReadOnlyCollection<SecurityKey>> GetSigningKeysAsync(string url, CancellationToken cancellationToken)
    {
        var json = await GetCachedDocumentAsync(url, "auth:jwks:", cancellationToken);
        return new JsonWebKeySet(json).GetSigningKeys().ToArray();
    }

    private async Task<string> GetCachedDocumentAsync(
        string url,
        string cachePrefix,
        CancellationToken cancellationToken,
        bool storeDownloaded = true)
    {
        var cacheKey = cachePrefix + url;
        string? json = null;
        try
        {
            json = await cache.GetAsync<string>(cacheKey);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "OIDC document cache read failed for {Url}.", url);
        }

        if (!string.IsNullOrWhiteSpace(json))
            return json;

        var client = httpClientFactory.CreateClient(HttpClientName);
        json = await client.GetStringAsync(url, cancellationToken);
        if (storeDownloaded)
            await StoreCachedDocumentAsync(url, cachePrefix, json);

        return json;
    }

    private async Task StoreCachedDocumentAsync(string url, string cachePrefix, string json)
    {
        try
        {
            await cache.SetAsync(cachePrefix + url, json, TimeSpan.FromHours(6));
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "OIDC document cache write failed for {Url}.", url);
        }
    }

    private static string MicrosoftKeysUrl(string? tenant)
    {
        var value = string.IsNullOrWhiteSpace(tenant) ? "common" : tenant.Trim();
        return $"https://login.microsoftonline.com/{value}/discovery/v2.0/keys";
    }

    private static string ComputeAppSecretProof(string accessToken, string appSecret)
    {
        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(appSecret), Encoding.UTF8.GetBytes(accessToken));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
