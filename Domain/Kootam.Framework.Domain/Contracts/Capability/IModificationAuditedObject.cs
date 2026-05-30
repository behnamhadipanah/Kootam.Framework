namespace Kootam.Framework.Domain.Contracts.Capability;

public interface IModificationAuditedObject<TKey>
{
    DateTime? LastModificationTime { get; }
    TKey? LastModifierId { get; }
    void SetLastModification(DateTime dateTime);
    void SetLastModifier(TKey userId);
}


