using System.Text.Json;
using InquirySample.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace InquirySample.Infrastructure.Services;

public sealed class RedisService(IDistributedCache cache) : IRedisService
{

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await cache.GetStringAsync(key, cancellationToken);

        if (json is null)
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task WriteAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expiration,
        };

        await cache.SetStringAsync(key, json, options, cancellationToken);
    }
}