using System.Collections.Concurrent;

namespace InquirySample.Infrastruture.Services.Inquiry;

public sealed class HsCodeLockProvider
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public SemaphoreSlim Get(string hsCode) =>
        _locks.GetOrAdd(hsCode, _ => new SemaphoreSlim(1, 1));
}
