namespace Kootam.Extensions.Caching.Abstractions;

public interface ICacheAdapter : 
    ICacheStore,
    ICacheBulkOperations,
    ICacheExpiration,
    ICacheGetOrSet,
    ICachePatternRemoval,
    ICacheGetOrRefresh
{
}