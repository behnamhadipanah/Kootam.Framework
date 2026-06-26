namespace Kootam.Framework.Domain.Contracts.Capability;

public interface IFullAuditedAggregateRoot<TKey> : ICreationAuditedObject<TKey>, IModificationAuditedObject<TKey>, ISoftDelete
{

}