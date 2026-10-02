namespace Journey_of_faith.Application.common.interfaces.caching;


public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
    void Remove(string key);
}