using System.Text.Encodings.Web;
using Kootam.Extensions.Authentication.Abstractions.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kootam.Extensions.Authentication.Handlers;

public class KootamAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    IEnumerable<IAuthHandler> handlers)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        foreach (var handler in handlers)
        {
            var result = await handler.AuthenticateAsync(Context);
            if (result.Succeeded)
            {
                return AuthenticateResult.Success(
                    new AuthenticationTicket(
                        result.Principal,
                        handler.Scheme       
                    )
                );
            }
        }

        return AuthenticateResult.NoResult();
    }
}

