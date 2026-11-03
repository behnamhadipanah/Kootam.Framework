namespace Kootam.Sms;

public enum SmsDeliveryFailureKind
{
    Configuration = 1,
    InvalidParameters = 2,
    Network = 3,
    Timeout = 4,
    AuthenticationRejected = 5,
    AccountRestricted = 6,
    InsufficientCredit = 7,
    ProviderRejected = 8,
    UnexpectedResponse = 9
}

public sealed class SmsDeliveryException : Exception
{
    public SmsDeliveryException(SmsDeliveryFailureKind kind, string message, int? providerStatus = null)
        : base(message)
    {
        Kind = kind;
        ProviderStatus = providerStatus;
    }

    public SmsDeliveryFailureKind Kind { get; }

    public int? ProviderStatus { get; }
}
