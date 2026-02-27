namespace Kootam.Framework.Domain.Interfaces.Capability;

public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTime? DeletionTime { get; }

    void Delete(DateTime now);
    void Restore();
}
