namespace Kootam.Sms;

public interface ISmsOtpSender
{
    Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
}

public interface ISmsSender : ISmsOtpSender
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
