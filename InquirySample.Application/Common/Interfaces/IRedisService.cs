namespace InquirySample.Application.Common.Interfaces;


public interface IRedisService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task WriteAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);
}
