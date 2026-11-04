using Kootam.Authentication.Abstractions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.SignIn.Configuration;

internal sealed class ExternalSignInStartupLogger(
    IOptions<SignInOptions> options,
    ILogger<ExternalSignInStartupLogger> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var warning in SignInConfigurationRules.ExternalWarnings(options.Value.External))
            logger.LogWarning("{ExternalSignInConfiguration}", warning);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
