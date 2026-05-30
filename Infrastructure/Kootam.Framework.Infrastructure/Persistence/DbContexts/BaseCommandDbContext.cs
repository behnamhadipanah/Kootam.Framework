using Kootam.Framework.Domain.ValueObjects;
using Kootam.Framework.Infrastructure.ValueConversions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Kootam.Framework.Infrastructure.Persistence.DbContexts;
public class BaseCommandDbContext<TDbContext>(DbContextOptions<TDbContext> options) : BaseDbContext<TDbContext>(options)
    where TDbContext : DbContext
{

    public T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
    {
        var value = Entry(entity).Property(propertyName).CurrentValue;
        return value != null
            ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
            : default;
    }

    public object GetShadowPropertyValue(object entity, string propertyName)
    {
        return Entry(entity).Property(propertyName).CurrentValue;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<BusinessId>().HaveConversion<BusinessIdConversion>();

    }

    
}
