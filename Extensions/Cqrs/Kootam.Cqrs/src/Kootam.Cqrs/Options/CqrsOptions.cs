using System.Reflection;

namespace Kootam.Cqrs.Options;

public class CqrsOptions
{
    public bool EnableLogging { get; set; } = true;
    public bool EnableValidation { get; set; } = true;
    public bool EnableDomainExceptionHandling { get; set; } = true;

    internal List<Assembly> Assemblies { get; } = new();
    public void RegisterServicesFromAssembly(Assembly assembly)
    {
        Assemblies.Add(assembly);
    }

    public void RegisterServicesFromAssemblyContaining<T>()
    {
        Assemblies.Add(typeof(T).Assembly);
    }
}