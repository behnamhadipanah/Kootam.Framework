namespace Kootam.Framework.Domain.Interfaces.Capability;

public interface IHasConcurrencyStamp
{
    string ConcurrencyStamp { get; }
}


