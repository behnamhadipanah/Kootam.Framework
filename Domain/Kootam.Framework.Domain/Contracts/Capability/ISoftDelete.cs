namespace Kootam.Framework.Domain.Contracts.Capability;

public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTime? DeletionTime { get; }

    void Delete(DateTime now);
    void Restore();
}
