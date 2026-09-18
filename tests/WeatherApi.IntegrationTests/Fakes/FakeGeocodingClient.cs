using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;

namespace WeatherApi.IntegrationTests.Fakes;

/// <summary>
/// Deterministic stand-in for the real Nominatim client so integration tests
/// never hit the network.
/// </summary>
public sealed class FakeGeocodingClient : IGeocodingClient
{
    public Task<Coordinates?> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        Coordinates? result = address switch
        {
            "Alès" => new Coordinates(44.1253665, 4.0852818),
            _ => null,
        };
        return Task.FromResult(result);
    }
}
