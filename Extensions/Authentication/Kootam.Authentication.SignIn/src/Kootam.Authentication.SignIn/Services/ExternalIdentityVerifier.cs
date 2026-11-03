using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Kootam.Authentication.SignIn.Contracts;
using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Kootam.Authentication.SignIn.Contracts.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kootam.Authentication.SignIn.Services;

public sealed class ExternalIdentityVerifier(
    IOptions<SignInOptions> options,
    IHttpClientFactory httpClientFactory,
    ISignInCache cache,
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
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        var keys = await GetSigningKeysAsync(jwksUrl, cancellationToken);
        var document = await OidcIdTokenValidator.ValidateAsync(idToken, keys, clientIds, issuerRule);
        if (document is null)
            return ExternalVerificationResult.Fail(ExternalVerificationStatus.Failed);

        var email = string.IsNullOrWhiteSpace(document.Email) ? null : document.Email.Trim();
        return ExternalVerificationResult.Success(new ExternalIdentity(
            provider,
            document.Subject,
            email,
            email is not null && document.EmailVerified,
            document.GivenName,
            document.FamilyName,
            null));
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

    private async Task<IReadOnlyCollection<SecurityKey>> GetSigningKeysAsync(string url, CancellationToken cancellationToken)
    {
        var cacheKey = "auth:jwks:" + url;
        string? json = null;
        try
        {
            json = await cache.GetAsync<string>(cacheKey);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "JWKS cache read failed for {Url}.", url);
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            var client = httpClientFactory.CreateClient(HttpClientName);
            json = await client.GetStringAsync(url, cancellationToken);
            try
            {
                await cache.SetAsync(cacheKey, json, TimeSpan.FromHours(6));
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "JWKS cache write failed for {Url}.", url);
            }
        }

        return new JsonWebKeySet(json).GetSigningKeys().ToArray();
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
