using System.Security.Cryptography;
using Kootam.Authentication.SignIn.Configuration;
using Kootam.Authentication.Abstractions.Enums;
using Kootam.Authentication.Abstractions.Models;
using Kootam.Authentication.Abstractions.Options;
using Kootam.Authentication.SignIn.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Kootam.Authentication.SignIn.UnitTest;

public class ExternalIdentityVerifierTests
{
    [Fact]
    public async Task Google_apple_and_microsoft_accept_a_signed_id_token()
    {
        var tenant = "11111111-1111-1111-1111-111111111111";
        var google = await VerifyOidc(
            ExternalSignInProvider.Google,
            "https://accounts.google.com",
            "web.apps.googleusercontent.com",
            "https://www.googleapis.com/oauth2/v3/certs");
        var apple = await VerifyOidc(
            ExternalSignInProvider.Apple,
            "https://appleid.apple.com",
            "com.example.app",
            "https://appleid.apple.com/auth/keys");
        var microsoft = await VerifyOidc(
            ExternalSignInProvider.Microsoft,
            $"https://login.microsoftonline.com/{tenant}/v2.0",
            "microsoft-client",
            $"https://login.microsoftonline.com/{tenant}/discovery/v2.0/keys",
            tenant);

        Assert.Equal("user-1", google.Identity!.Subject);
        Assert.Equal("ada@example.com", google.Identity.Email);
        Assert.True(google.Identity.EmailVerified);
        Assert.Equal("user-1", apple.Identity!.Subject);
        Assert.Equal("user-1", microsoft.Identity!.Subject);
    }

    [Fact]
    public async Task Oidc_accepts_keycloak_by_default_and_any_other_discovered_issuer()
    {
        const string keycloak = "https://auth.example.com/realms/app";
        var keycloakIdentity = await VerifyOidc(
            ExternalSignInProvider.Oidc,
            keycloak,
            "web",
            $"{keycloak}/protocol/openid-connect/certs",
            configuredAuthority: $"{keycloak}/",
            preferredUserName: "ada",
            authorizedParty: "web");
        var otherIdentity = await VerifyOidc(
            ExternalSignInProvider.Oidc,
            "https://idp.example.com",
            "mobile",
            "https://cdn.example.com/jwks",
            preferredUserName: "ada");
        var foreignIssuer = await VerifyOidc(
            ExternalSignInProvider.Oidc,
            "https://auth.example.com/realms/other",
            "web",
            $"{keycloak}/protocol/openid-connect/certs",
            configuredAuthority: keycloak,
            expectSuccess: false);
        var foreignClient = await VerifyOidc(
            ExternalSignInProvider.Oidc,
            keycloak,
            "web",
            $"{keycloak}/protocol/openid-connect/certs",
            authorizedParty: "other-client",
            expectSuccess: false);
        var foreignDiscovery = await VerifyOidc(
            ExternalSignInProvider.Oidc,
            keycloak,
            "web",
            $"{keycloak}/protocol/openid-connect/certs",
            discoveredIssuer: "https://evil.example",
            expectSuccess: false);

        Assert.Equal("user-1", keycloakIdentity.Identity!.Subject);
        Assert.Equal("ada", keycloakIdentity.Identity.SuggestedUserName);
        Assert.True(keycloakIdentity.Identity.EmailVerified);
        Assert.Equal("user-1", otherIdentity.Identity!.Subject);
        Assert.Equal(ExternalVerificationStatus.Failed, foreignIssuer.Status);
        Assert.Equal(ExternalVerificationStatus.Failed, foreignClient.Status);
        Assert.Equal(ExternalVerificationStatus.Failed, foreignDiscovery.Status);
    }

    [Fact]
    public async Task Facebook_accepts_a_valid_access_token()
    {
        var handler = new StubHandler(request =>
        {
            var url = request.RequestUri!.AbsoluteUri;
            var json = url.Contains("debug_token", StringComparison.Ordinal)
                ? """{"data":{"app_id":"app-id","is_valid":true}}"""
                : """{"id":"fb-1","email":"ada@example.com","first_name":"Ada","last_name":"Lovelace"}""";
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        });
        var verifier = Create(handler, options =>
        {
            options.External.Facebook.Enabled = true;
            options.External.Facebook.AppId = "app-id";
            options.External.Facebook.AppSecret = "app-secret";
        });

        var result = await verifier.VerifyAsync(new ExternalSignInRequest(
            ExternalSignInProvider.Facebook,
            AccessToken: "user-token"));

        Assert.True(result.Succeeded);
        Assert.Equal("fb-1", result.Identity!.Subject);
        Assert.True(result.Identity.EmailVerified);
    }

    [Fact]
    public async Task Telegram_accepts_a_valid_widget_payload_and_rejects_a_bad_hash()
    {
        var now = DateTimeOffset.FromUnixTimeSeconds(1_700_000_000);
        var data = new TelegramLoginData(10, "Ada", "Lovelace", "ada", null, now.ToUnixTimeSeconds(), "");
        var hash = TelegramLoginAuthenticator.ComputeHash(data, "123:token");
        var valid = data with { Hash = hash };
        var verifier = Create(new StubHandler(_ => new HttpResponseMessage(System.Net.HttpStatusCode.OK)), options =>
        {
            options.External.Telegram.Enabled = true;
            options.External.Telegram.BotToken = "123:token";
        }, now);

        var accepted = await verifier.VerifyAsync(new ExternalSignInRequest(ExternalSignInProvider.Telegram, Telegram: valid));
        var rejected = await verifier.VerifyAsync(new ExternalSignInRequest(
            ExternalSignInProvider.Telegram,
            Telegram: valid with { Hash = "00" }));

        Assert.True(accepted.Succeeded);
        Assert.Equal("10", accepted.Identity!.Subject);
        Assert.Equal("ada", accepted.Identity.SuggestedUserName);
        Assert.Equal(ExternalVerificationStatus.Failed, rejected.Status);
    }

    [Fact]
    public async Task Unconfigured_provider_is_disabled()
    {
        var verifier = Create(new StubHandler(_ => throw new InvalidOperationException("network")), _ => { });
        var result = await verifier.VerifyAsync(new ExternalSignInRequest(ExternalSignInProvider.Google, IdToken: "token"));
        Assert.Equal(ExternalVerificationStatus.Disabled, result.Status);
    }

    private static async Task<ExternalVerificationResult> VerifyOidc(
        ExternalSignInProvider provider,
        string issuer,
        string audience,
        string jwksUrl,
        string? tenant = null,
        string? configuredAuthority = null,
        string? preferredUserName = null,
        string? authorizedParty = null,
        string? discoveredIssuer = null,
        bool expectSuccess = true)
    {
        using var rsa = RSA.Create(2048);
        var key = new RsaSecurityKey(rsa) { KeyId = "k1" };
        var parameters = rsa.ExportParameters(false);
        var jwks = $$"""
            {"keys":[{"kty":"RSA","use":"sig","kid":"k1","alg":"RS256","n":"{{Base64UrlEncoder.Encode(parameters.Modulus)}}","e":"{{Base64UrlEncoder.Encode(parameters.Exponent)}}"}]}
            """;
        var handler = new StubHandler(request =>
        {
            var path = request.RequestUri!.GetLeftPart(UriPartial.Path);
            if (provider == ExternalSignInProvider.Oidc
                && path.EndsWith("/.well-known/openid-configuration", StringComparison.Ordinal))
            {
                var authority = (configuredAuthority ?? issuer).Trim().TrimEnd('/');
                Assert.Equal($"{authority}/.well-known/openid-configuration", path);
                var published = discoveredIssuer ?? authority;
                var discovery = $$"""{"issuer":"{{published}}","jwks_uri":"{{jwksUrl}}"}""";
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(discovery, System.Text.Encoding.UTF8, "application/json")
                };
            }

            Assert.Equal(jwksUrl, path);
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(jwks)
            };
        });
        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = "user-1",
            [JwtRegisteredClaimNames.Email] = "ada@example.com",
            ["email_verified"] = true,
            [JwtRegisteredClaimNames.GivenName] = "Ada",
            [JwtRegisteredClaimNames.FamilyName] = "Lovelace"
        };
        if (tenant is not null)
            claims["tid"] = tenant;
        if (preferredUserName is not null)
            claims[JwtRegisteredClaimNames.PreferredUsername] = preferredUserName;
        if (authorizedParty is not null)
            claims["azp"] = authorizedParty;

        var token = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            NotBefore = DateTime.UtcNow.AddMinutes(-1),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Claims = claims,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
        });

        var verifier = Create(handler, options =>
        {
            switch (provider)
            {
                case ExternalSignInProvider.Google:
                    options.External.Google.Enabled = true;
                    options.External.Google.ClientId = audience;
                    break;
                case ExternalSignInProvider.Apple:
                    options.External.Apple.Enabled = true;
                    options.External.Apple.ClientId = audience;
                    break;
                case ExternalSignInProvider.Microsoft:
                    options.External.Microsoft.Enabled = true;
                    options.External.Microsoft.ClientId = audience;
                    options.External.Microsoft.Tenant = tenant!;
                    break;
                case ExternalSignInProvider.Oidc:
                    options.External.Oidc.Enabled = true;
                    options.External.Oidc.ClientId = audience;
                    options.External.Oidc.Authority = configuredAuthority ?? issuer;
                    break;
            }
        });

        var result = await verifier.VerifyAsync(new ExternalSignInRequest(provider, IdToken: token));
        if (expectSuccess)
            Assert.True(result.Succeeded);
        else
            Assert.False(result.Succeeded);

        return result;
    }

    private static ExternalIdentityVerifier Create(
        HttpMessageHandler handler,
        Action<SignInOptions> configure,
        DateTimeOffset? utcNow = null)
    {
        var options = new SignInOptions();
        configure(options);
        var client = new HttpClient(handler);
        return new ExternalIdentityVerifier(
            Options.Create(options),
            new SingleClientFactory(client),
            new MemorySignInCache(),
            utcNow is null ? TimeProvider.System : new FixedTimeProvider(utcNow.Value),
            NullLogger<ExternalIdentityVerifier>.Instance);
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(responder(request));
    }

    private sealed class SingleClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}

public class SignInConfigurationRulesTests
{
    [Fact]
    public void Enabled_google_without_a_web_client_id_is_a_warning()
    {
        var external = new ExternalSignInOptions();
        external.Google.Enabled = true;
        external.Google.ClientId = "GOCSPX-secret";

        var warnings = SignInConfigurationRules.ExternalWarnings(external).ToArray();
        Assert.Contains(warnings, warning => warning.Contains("client secret", StringComparison.Ordinal));
    }

    [Fact]
    public void Enabled_oidc_without_an_authority_is_a_warning()
    {
        var external = new ExternalSignInOptions();
        external.Oidc.Enabled = true;
        external.Oidc.ClientId = "web";

        var warnings = SignInConfigurationRules.ExternalWarnings(external).ToArray();
        Assert.Contains(warnings, warning => warning.Contains("SignIn:External:Oidc:Authority", StringComparison.Ordinal));
    }

    [Fact]
    public void Catalog_publishes_the_oidc_authority_and_client_id()
    {
        var options = new SignInOptions();
        options.External.Oidc.Enabled = true;
        options.External.Oidc.Authority = "https://auth.example.com/realms/app/";
        options.External.Oidc.ClientId = "web";
        options.External.Oidc.RedirectUri = "https://app.example/callback";

        var description = new ExternalSignInCatalog(Options.Create(options)).Describe().Single();

        Assert.Equal("Oidc", description.Name);
        Assert.Equal("web", description.ClientId);
        Assert.Equal("https://auth.example.com/realms/app", description.Authority);
        Assert.Equal("https://app.example/callback", description.RedirectUri);
    }
}
