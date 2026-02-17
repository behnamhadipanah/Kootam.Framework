namespace Kootam.Framework.Domain.Interfaces.Capability;

public interface ICreationAuditedObject<TKey>
{
    DateTime CreationTime { get; }
    TKey CreatorId { get; }
}


