namespace Kootam.Extensions.Caching.Abstractions;

public interface ICachePatternRemoval
{
    Task<long> RemoveByPatternAsync(string pattern);
}