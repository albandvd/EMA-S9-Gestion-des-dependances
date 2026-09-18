using WeatherApi.Domain;

namespace WeatherApi.Application.Interfaces;

/// <summary>
/// Abstraction over a geocoding provider. Implementations live in Infrastructure
/// and must never leak HTTP concerns to callers.
/// </summary>
public interface IGeocodingClient
{
    /// <summary>
    /// Resolves an address to coordinates, or null if no location matches.
    /// </summary>
    Task<Coordinates?> GeocodeAsync(string address, CancellationToken cancellationToken);
}
