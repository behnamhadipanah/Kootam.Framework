using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Kootam.Email;

public static class SmtpServerCertificatePolicy
{
    public static bool Accept(string expectedHost, X509Certificate? certificate, SslPolicyErrors errors)
    {
        if (errors == SslPolicyErrors.None)
            return true;

        if (certificate is null || string.IsNullOrWhiteSpace(expectedHost))
            return false;

        if ((errors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
            return false;

        // Some mail hosts publish a self-signed certificate for the configured hostname.
        // Accept that chain only when the certificate name is the configured SMTP host.
        if (errors != SslPolicyErrors.RemoteCertificateChainErrors)
            return false;

        var owned = certificate as X509Certificate2;
        var parsed = owned ?? new X509Certificate2(certificate);
        try
        {
            var dnsName = parsed.GetNameInfo(X509NameType.DnsName, false);
            return string.Equals(dnsName, expectedHost, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (owned is null)
                parsed.Dispose();
        }
    }
}
