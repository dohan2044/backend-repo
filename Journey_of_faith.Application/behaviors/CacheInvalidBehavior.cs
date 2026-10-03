using MediatR;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.caching.interfaces;
namespace Journey_of_faith.Application.behaviors;

public class CacheInvalidBehavior<TRequest, TResponse>(ICacheGroupVersionStore cacheGroupVersions) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, ICacheInvalidCommand
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
        if(request.CacheGroups is not null)
        {
            foreach(string cacheGroup in request.CacheGroups)
            {
                cacheGroupVersions.Invalidate(cacheGroup);
            }
        }

        return response;
    }
}