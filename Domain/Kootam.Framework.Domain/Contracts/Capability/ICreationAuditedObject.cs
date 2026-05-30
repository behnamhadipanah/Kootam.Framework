namespace Kootam.Framework.Domain.Contracts.Capability;

public interface ICreationAuditedObject<TKey>
{
    DateTime CreationTime { get; }
    TKey CreatorId { get; }
    void SetCreated(DateTime now);
    void SetCreator(TKey creatorId);

}


