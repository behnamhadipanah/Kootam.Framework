using Kootam.Authentication.SignIn.Contracts;
using Kootam.Authentication.SignIn.Contracts.Options;
using Kootam.Authentication.SignIn.Enums;
using Kootam.Authentication.SignIn.Models;
using Kootam.Email;
using Kootam.Sms;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.SignIn.Sample;

public sealed class ContactOtpSender(
    IOptions<SignInOptions> signIn,
    IOptions<EmailOptions> email,
    IOptions<SmsOptions> sms,
    IEmailDocumentRenderer renderer,
    IEmailSender emailSender,
    ISmsOtpSender smsOtpSender,
    SignInDevelopmentCodes developmentCodes) : IContactOtpSender
{
    public async Task SendAsync(
        ContactOtpPurpose purpose,
        string destination,
        string code,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default)
    {
        if (purpose == ContactOtpPurpose.MobileLogin)
        {
            await smsOtpSender.SendOtpAsync(destination, code, cancellationToken);
            if (sms.Value.IsLog)
                developmentCodes.Remember(purpose, code);
            return;
        }

        var method = signIn.Value.EmailOtp;
        var minutes = Math.Max(1, (int)Math.Ceiling((expiresAt - DateTimeOffset.UtcNow).TotalMinutes));
        var document = renderer.Render(new EmailCard(
            Subject: method.Subject,
            LogoUrl: email.Value.LogoUrl,
            Body: method.Body,
            Title: method.Subject,
            Highlight: code,
            Footer: string.IsNullOrWhiteSpace(method.Footer)
                ? $"This code expires in {minutes} minutes."
                : method.Footer));
        await emailSender.SendAsync(destination, document, cancellationToken);
        if (email.Value.IsLog)
            developmentCodes.Remember(purpose, code);
    }
}
