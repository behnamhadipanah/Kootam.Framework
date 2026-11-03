using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Kavenegar;
using Kavenegar.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kootam.Sms;

public sealed class KavenegarSmsSender(
    IHttpClientFactory httpClientFactory,
    IOptions<SmsOptions> options,
    ILogger<KavenegarSmsSender> logger) : ISmsSender
{
    public const string HttpClientName = "KootamKavenegar";

    public Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        var receptor = Encode(RequireReceptor(phoneNumber));
        var token = Encode(RequireToken(code));
        var template = Encode(RequireSetting(options.Value.Kavenegar.OtpTemplate, "Sms:Kavenegar:OtpTemplate"));
        return SendCoreAsync(
            api => api.VerifyLookupAsync(receptor, token, template),
            "VerifyLookup",
            cancellationToken);
    }

    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        var settings = options.Value.Kavenegar;
        var sender = RequireSetting(settings.Sender, "Sms:Kavenegar:Sender");
        var receptor = RequireReceptor(phoneNumber);
        var text = RequireMessage(message);
        return SendCoreAsync(
            api => api.SendAsync(sender, receptor, text),
            "Send",
            cancellationToken);
    }

    private async Task SendCoreAsync(
        Func<KavenegarApi, Task> send,
        string operation,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var transport = CreateTransport(options.Value.Kavenegar);
        var api = new KavenegarApi(
            new KavenegarOptions
            {
                ApiKey = transport.ApiKey,
                BaseUrl = transport.SdkBaseUrl
            },
            transport.HttpClient);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await send(api);
            stopwatch.Stop();
            logger.LogInformation(
                "Kavenegar {Operation} accepted. ElapsedMs={ElapsedMs}",
                operation,
                stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw new SmsDeliveryException(SmsDeliveryFailureKind.Timeout, $"Kavenegar {operation} timed out.");
        }
        catch (ApiException exception)
        {
            var status = (int)exception.Code;
            var kind = Classify(status);
            logger.LogWarning(
                "Kavenegar {Operation} rejected. Kind={Kind} ProviderStatus={ProviderStatus} ElapsedMs={ElapsedMs}",
                operation,
                kind,
                status,
                stopwatch.ElapsedMilliseconds);
            throw new SmsDeliveryException(
                kind,
                $"Kavenegar {operation} was rejected ({kind}, status {status}).",
                status);
        }
        catch (HttpException)
        {
            throw new SmsDeliveryException(
                SmsDeliveryFailureKind.Network,
                $"Kavenegar {operation} could not reach the provider.");
        }
        catch (SmsDeliveryException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new SmsDeliveryException(
                SmsDeliveryFailureKind.UnexpectedResponse,
                $"Kavenegar {operation} returned an unexpected response.");
        }
    }

    private Transport CreateTransport(KavenegarSmsOptions settings)
    {
        var apiKey = RequireSetting(settings.ApiKey, "Sms:Kavenegar:ApiKey");
        var baseUrl = RequireSetting(settings.BaseUrl, "Sms:Kavenegar:BaseUrl");
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.Configuration, "Sms:Kavenegar:BaseUrl is not an absolute URL.");

        var pooled = httpClientFactory.CreateClient(HttpClientName);
        pooled.Timeout = TimeSpan.FromSeconds(Math.Max(1, settings.TimeoutSeconds));
        var handler = new KavenegarBusinessStatusHandler
        {
            InnerHandler = new PooledClientHandler(pooled)
        };
        return new Transport(apiKey, ToSdkBaseUrl(baseUrl), new HttpClient(handler, disposeHandler: true));
    }

    // The SDK appends /v1/{apiKey}/... onto BaseUrl. The app setting already includes /v1.
    private static string ToSdkBaseUrl(string configuredBaseUrl)
    {
        var trimmed = configuredBaseUrl.Trim().TrimEnd('/');
        if (trimmed.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
            return trimmed[..^3].TrimEnd('/');

        return trimmed;
    }

    // VerifyLookup writes form fields without encoding. Encode first so a leading '+' is not read as a space.
    private static string Encode(string value) => WebUtility.UrlEncode(value);

    private static string RequireReceptor(string phoneNumber)
    {
        if (!PhoneNumberNormalizer.IsValid(phoneNumber))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.InvalidParameters, "Kavenegar receptor is not a valid phone number.");

        return PhoneNumberNormalizer.Normalize(phoneNumber)!;
    }

    private static string RequireToken(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length > 100 || code.Any(character => !char.IsAsciiLetterOrDigit(character)))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.InvalidParameters, "Kavenegar token is empty or contains unsupported characters.");

        return code.Trim();
    }

    private static string RequireMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.InvalidParameters, "Kavenegar message is empty.");

        return message.Trim();
    }

    private static string RequireSetting(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.Configuration, $"{name} is required when SMS delivery uses Kavenegar.");

        return value.Trim();
    }

    private static SmsDeliveryFailureKind Classify(int status)
        => status switch
        {
            401 => SmsDeliveryFailureKind.AccountRestricted,
            403 => SmsDeliveryFailureKind.AuthenticationRejected,
            418 => SmsDeliveryFailureKind.InsufficientCredit,
            411 or 412 or 413 or 422 or 424 or 426 or 427 or 428 or 431 or 432 => SmsDeliveryFailureKind.InvalidParameters,
            400 or 402 or 404 or 405 or 406 or 407 or 409 or 414 or 415 or 416 or 417 or 419 or 420 => SmsDeliveryFailureKind.ProviderRejected,
            _ => SmsDeliveryFailureKind.UnexpectedResponse
        };

    private sealed class Transport(string apiKey, string sdkBaseUrl, HttpClient httpClient) : IDisposable
    {
        public string ApiKey { get; } = apiKey;

        public string SdkBaseUrl { get; } = sdkBaseUrl;

        public HttpClient HttpClient { get; } = httpClient;

        public void Dispose() => HttpClient.Dispose();
    }

    private sealed class PooledClientHandler(HttpClient pooled) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using var copy = await CopyAsync(request, cancellationToken);
            return await pooled.SendAsync(copy, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
        }

        private static async Task<HttpRequestMessage> CopyAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var copy = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
                copy.Headers.TryAddWithoutValidation(header.Key, header.Value);

            if (request.Content is null)
                return copy;

            var body = await request.Content.ReadAsStringAsync(cancellationToken);
            copy.Content = new StringContent(body);
            if (request.Content.Headers.ContentType is not null)
                copy.Content.Headers.ContentType = request.Content.Headers.ContentType;

            return copy;
        }
    }

    private sealed class KavenegarBusinessStatusHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode || response.Content is null)
                return response;

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            response.Content = new StringContent(body);
            if (!string.IsNullOrWhiteSpace(mediaType))
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);

            if (TryReadReturnStatus(body, out var status) && status != 200)
                response.StatusCode = (HttpStatusCode)status;

            return response;
        }

        private static bool TryReadReturnStatus(string body, out int status)
        {
            status = 0;
            if (string.IsNullOrWhiteSpace(body))
                return false;

            try
            {
                using var document = JsonDocument.Parse(body);
                if (!document.RootElement.TryGetProperty("return", out var envelope)
                    || !envelope.TryGetProperty("status", out var statusElement)
                    || statusElement.ValueKind != JsonValueKind.Number
                    || !statusElement.TryGetInt32(out status))
                    return false;

                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
