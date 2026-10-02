namespace Journey_of_faith.Application.common.caching.interfaces;

public interface ICacheableQuery
{
    string CacheKey {get; }
    TimeSpan? Expiration {get; }
}