using MediatR;
using Journey_of_faith.Application.common.caching.interfaces;
using Journey_of_faith.Application.common.interfaces.caching;
namespace Journey_of_faith.Application.behaviors;

public class CachingBehavior<TRequest, TResponse>(
    ICacheService _cache,
    ICacheGroupVersionStore _cacheGroupVersions) : 
    IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = $"{request.CacheGroup}:v{_cacheGroupVersions.GetVersion(request.CacheGroup)}:{request.CacheKey}";
        var cachedResponse = _cache.Get<TResponse>(cacheKey);
        if(cachedResponse != null)
        {
            return cachedResponse;
        }
        
        // neu chua cos du lieu thi goi tiep handle de lay du lieu tu database
        var response = await next();
        if(response != null)
        {
            // luu du lieu vao cho lan sau goi
            _cache.Set(cacheKey, response, request.Expiration);
        }
        return response;
    }
}