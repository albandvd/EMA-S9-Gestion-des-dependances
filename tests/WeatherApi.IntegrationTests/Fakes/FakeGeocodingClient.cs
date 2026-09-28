using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;

namespace WeatherApi.IntegrationTests.Fakes;

/// <summary>
/// Deterministic stand-in for the real Nominatim client so integration tests
/// never hit the network. Counts calls so tests can assert on caching behavior.
/// </summary>
public sealed class FakeGeocodingClient : IGeocodingClient
{
    public int CallCount { get; private set; }

    public Task<Coordinates?> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        CallCount++;

        Coordinates? result = address switch
        {
            "Alès" => new Coordinates(44.1253665, 4.0852818),
            "Nîmes" => new Coordinates(43.8367, 4.3601),
            _ => null,
        };
        return Task.FromResult(result);
    }
}
