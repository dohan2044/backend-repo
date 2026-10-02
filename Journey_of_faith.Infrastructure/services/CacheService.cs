using Journey_of_faith.Application.common.interfaces.caching;
using Microsoft.Extensions.Caching.Memory;

namespace Journey_of_faith.Infrastructure.services;

public class CacheService(IMemoryCache _cache) : ICacheService
{
    public T? Get<T>(string key) => _cache.TryGetValue(key, out T value) ? value : default;
    public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (absoluteExpiration.HasValue)
            options.SetAbsoluteExpiration(absoluteExpiration.Value);
        if (slidingExpiration.HasValue)
            options.SetSlidingExpiration(slidingExpiration.Value);
        _cache.Set(key, value, options);
    }
    public void Remove(string key) => _cache.Remove(key);
}