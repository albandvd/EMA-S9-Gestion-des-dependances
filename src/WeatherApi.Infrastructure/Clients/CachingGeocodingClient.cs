using Microsoft.Extensions.Options;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Infrastructure.Configuration;

namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Decorates an IGeocodingClient so two successive lookups for the same address
/// only trigger one real network call. Wraps the abstraction rather than any
/// concrete provider, so it applies to whichever geocoding provider is active.
/// </summary>
public sealed class CachingGeocodingClient(
    IGeocodingClient inner,
    IResponseCache cache,
    IOptions<CacheOptions> options) : IGeocodingClient
{
    public Task<Coordinates?> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        var key = $"geocoding:{address.Trim().ToLowerInvariant()}";
        return cache.GetOrCreateAsync(
            key,
            options.Value.GeocodingTtl,
            ct => inner.GeocodeAsync(address, ct),
            cancellationToken);
    }
}
