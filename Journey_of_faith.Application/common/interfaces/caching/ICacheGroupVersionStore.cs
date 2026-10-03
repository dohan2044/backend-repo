namespace Journey_of_faith.Application.common.caching.interfaces;

public interface ICacheGroupVersionStore
{
    long GetVersion(string cacheGroup);
    void Invalidate(string cacheGroup);
}
