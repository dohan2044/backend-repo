using System.Collections.Concurrent;
using Journey_of_faith.Application.common.caching.interfaces;

namespace Journey_of_faith.Application.common.caching;

public sealed class CacheGroupVersionStore : ICacheGroupVersionStore
{
    private readonly ConcurrentDictionary<string, long> _versions = new(StringComparer.Ordinal);

    public long GetVersion(string cacheGroup)
        => _versions.GetOrAdd(cacheGroup, 0);

    public void Invalidate(string cacheGroup)
        => _versions.AddOrUpdate(cacheGroup, 1, (_, version) => checked(version + 1));
}
