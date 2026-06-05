namespace Kootam.Caching.Abstractions;

public interface ICacheAdapter : 
    ICacheStore,
    ICacheBulkOperations,
    ICacheExpiration,
    ICacheGetOrSet,
    ICachePatternRemoval,
    ICacheGetOrRefresh
{
}