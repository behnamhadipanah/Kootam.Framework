namespace Kootam.Email;

public interface IEmailSender
{
    Task SendAsync(string to, EmailDocument document, CancellationToken cancellationToken = default);
}
