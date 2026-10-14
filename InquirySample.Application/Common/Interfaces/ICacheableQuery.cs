namespace InquirySample.Application.Common.Interfaces;

public interface ICacheableQuery<TResponse> : IRequest<ErrorOr<TResponse>>
{
    string CacheKey { get; }
}
