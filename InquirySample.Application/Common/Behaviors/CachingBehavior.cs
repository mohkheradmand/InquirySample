using InquirySample.Application.Common.Interfaces;

namespace InquirySample.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    IRedisService redisService) : IPipelineBehavior<TRequest, ErrorOr<TResponse>>
    where TRequest : ICacheableQuery<TResponse>, IRequest<ErrorOr<TResponse>>
{
    public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken cancellationToken)
    {
        var key = request.CacheKey;

        var cached = await redisService.GetAsync<TResponse>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var response = await next(cancellationToken);

        if (!response.IsError && response.Value is not null)
            await redisService.WriteAsync(key, response.Value, TimeSpan.FromMilliseconds(500), cancellationToken);

        return response;
    }
}
