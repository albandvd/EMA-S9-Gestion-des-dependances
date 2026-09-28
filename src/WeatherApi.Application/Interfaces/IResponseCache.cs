namespace WeatherApi.Application.Interfaces;

/// <summary>
/// Minimal cache abstraction so callers never depend on a specific caching
/// technology (in-memory today, a distributed cache tomorrow) — only on this
/// interface. No static state: the implementation's lifetime/storage is a DI
/// concern, not the caller's.
/// </summary>
public interface IResponseCache
{
    /// <summary>
    /// Returns the cached value for <paramref name="key"/>, or invokes
    /// <paramref name="factory"/> once, caches its result for <paramref name="ttl"/>,
    /// and returns it.
    /// </summary>
    Task<TValue> GetOrCreateAsync<TValue>(
        string key,
        TimeSpan ttl,
        Func<CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken);
}
