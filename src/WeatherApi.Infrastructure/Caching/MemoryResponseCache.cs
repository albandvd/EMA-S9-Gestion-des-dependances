using Microsoft.Extensions.Caching.Memory;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Infrastructure.Caching;

/// <summary>
/// IResponseCache backed by an in-process <see cref="IMemoryCache"/> (itself
/// registered as a DI singleton — not a static field). Swapping to a distributed
/// cache later only means providing a different IResponseCache implementation;
/// no caller changes.
/// </summary>
public sealed class MemoryResponseCache(IMemoryCache cache) : IResponseCache
{
    public async Task<TValue> GetOrCreateAsync<TValue>(
        string key,
        TimeSpan ttl,
        Func<CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(key, out TValue? cached))
        {
            return cached!;
        }

        var value = await factory(cancellationToken);
        cache.Set(key, value, ttl);
        return value;
    }
}
