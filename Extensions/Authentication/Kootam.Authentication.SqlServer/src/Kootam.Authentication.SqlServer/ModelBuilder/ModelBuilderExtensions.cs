namespace Kootam.Authentication.SqlServer.ModelBuilder;

public static class ModelBuilderExtensions
{
    public static Microsoft.EntityFrameworkCore.ModelBuilder AddRefreshToken(
        this Microsoft.EntityFrameworkCore.ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(ModelBuilderExtensions).Assembly);

        return builder;
    }
}