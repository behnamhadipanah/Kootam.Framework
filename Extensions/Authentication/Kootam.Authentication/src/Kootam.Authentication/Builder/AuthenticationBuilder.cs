using Microsoft.Extensions.DependencyInjection;

namespace Kootam.Authentication.Builder;

public sealed class AuthenticationBuilder
{
    public IServiceCollection Services { get; }

    internal AuthenticationBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
