using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kootam.Sms;

public sealed class LogSmsSender(IOptions<SmsOptions> options, ILogger<LogSmsSender> logger) : ISmsSender
{
    public Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var receptor = RequirePhone(phoneNumber);
        if (string.IsNullOrWhiteSpace(code))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.InvalidParameters, "SMS code is empty.");

        logger.LogInformation(
            "SMS code logged instead of delivered. Mode={Mode} Phone={Phone} Code={Code}",
            options.Value.Mode,
            receptor,
            code.Trim());
        return Task.CompletedTask;
    }

    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var receptor = RequirePhone(phoneNumber);
        if (string.IsNullOrWhiteSpace(message))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.InvalidParameters, "SMS message is empty.");

        logger.LogInformation(
            "SMS text logged instead of delivered. Mode={Mode} Phone={Phone} Length={Length}",
            options.Value.Mode,
            receptor,
            message.Trim().Length);
        return Task.CompletedTask;
    }

    private static string RequirePhone(string phoneNumber)
    {
        if (!PhoneNumberNormalizer.IsValid(phoneNumber))
            throw new SmsDeliveryException(SmsDeliveryFailureKind.InvalidParameters, "SMS receptor is not a valid phone number.");

        return PhoneNumberNormalizer.Normalize(phoneNumber)!;
    }
}
