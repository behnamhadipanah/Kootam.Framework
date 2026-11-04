using System.Net.Sockets;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Kootam.Email;

public sealed class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(string to, EmailDocument document, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentNullException.ThrowIfNull(document);

        var email = options.Value;
        if (!email.IsSmtp || !email.SmtpIsReady())
            throw new InvalidOperationException("Email:Mode is Smtp but the SMTP settings are incomplete.");

        var smtp = email.Smtp;
        var connectHost = string.IsNullOrWhiteSpace(smtp.ConnectHost) ? null : smtp.ConnectHost.Trim();
        var implicitSsl = smtp.UsesImplicitSsl();
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress(email.FromName?.Trim() ?? string.Empty, email.FromAddress!.Trim()));
        message.To.Add(MailboxAddress.Parse(to.Trim()));
        message.Subject = document.Subject;
        var alternative = new Multipart("alternative");
        alternative.Add(new TextPart("plain") { Text = document.PlainText });
        alternative.Add(new TextPart("html") { Text = document.Html });
        message.Body = alternative;

        using var client = new SmtpClient
        {
            Timeout = Math.Max(1, smtp.TimeoutSeconds) * 1000,
            CheckCertificateRevocation = false,
            ServerCertificateValidationCallback = (_, certificate, _, errors) =>
                SmtpServerCertificatePolicy.Accept(smtp.Host!.Trim(), certificate, errors)
        };

        var socketOptions = implicitSsl
            ? SecureSocketOptions.SslOnConnect
            : smtp.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        if (connectHost is null)
        {
            await client.ConnectAsync(smtp.Host!.Trim(), smtp.Port, socketOptions, cancellationToken);
        }
        else
        {
            await ConnectByAddressAsync(client, connectHost, smtp.Host!.Trim(), smtp.Port, socketOptions, cancellationToken);
        }

        await client.AuthenticateAsync(smtp.User!.Trim(), smtp.Password!, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
        logger.LogInformation(
            "SMTP message accepted. Host={Host} Port={Port}",
            smtp.Host,
            smtp.Port);
    }

    private static async Task ConnectByAddressAsync(
        SmtpClient client,
        string connectHost,
        string hostName,
        int port,
        SecureSocketOptions socketOptions,
        CancellationToken cancellationToken)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        var transferred = false;
        try
        {
            await socket.ConnectAsync(connectHost, port, cancellationToken);
            await client.ConnectAsync(socket, hostName, port, socketOptions, cancellationToken);
            transferred = true;
        }
        finally
        {
            if (!transferred)
                socket.Dispose();
        }
    }
}
