using Kootam.Authentication.Builder;

namespace Kootam.Authentication.Redis.DependencyInjection;

public static class RedisRefreshTokenExtensions
{

    public static AuthenticationBuilder AddRefreshTokenRedis(this AuthenticationBuilder builder)
    {
        
        return builder;
    }
    
}