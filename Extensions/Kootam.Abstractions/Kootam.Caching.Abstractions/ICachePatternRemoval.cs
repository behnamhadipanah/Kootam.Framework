namespace Kootam.Caching.Abstractions;

public interface ICachePatternRemoval
{
    Task<long> RemoveByPatternAsync(string pattern);
}