using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Journey_of_faith.Application.common.interfaces;
namespace Journey_of_faith.Application.behaviors;

public class CacheInvalidBehavior<TRequest, TResponse>(IMemoryCache memoryCache) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TRequest>, ICacheInvalidCommand
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        // chạy command trước sau đó thì mới xóa cache
        var response = await next();

        // nếu kh có lỗi thì xóa cache
        if(request.CacheKeys is not null)
        {
            foreach(string key in request.CacheKeys)
            {
                memoryCache.Remove(key);
            }
        }

        return response;
    }
}