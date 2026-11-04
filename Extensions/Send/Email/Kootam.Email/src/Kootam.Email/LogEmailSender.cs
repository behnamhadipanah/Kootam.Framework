using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kootam.Email;

public sealed class LogEmailSender(IOptions<EmailOptions> options, ILogger<LogEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string to, EmailDocument document, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentNullException.ThrowIfNull(document);

        logger.LogInformation(
            "Email logged instead of delivered. Mode={Mode} To={To} Subject={Subject} Body={Body}",
            options.Value.Mode,
            to.Trim(),
            document.Subject,
            document.PlainText);
        return Task.CompletedTask;
    }
}
