namespace Kootam.Framework.Abstractions;

public interface IHasId<TKey>
{
    TKey Id { get; }
}
