namespace Kootam.Framework.Domain.Contracts.Capability;

public interface IHasConcurrencyStamp
{
    string ConcurrencyStamp { get; }
}


