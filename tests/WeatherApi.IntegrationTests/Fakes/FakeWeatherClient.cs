using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;

namespace WeatherApi.IntegrationTests.Fakes;

/// <summary>
/// Deterministic stand-in for the real Open-Meteo client so integration tests
/// never hit the network.
/// </summary>
public sealed class FakeWeatherClient : IWeatherClient
{
    public Task<HourlyShortwaveRadiation> GetHourlyShortwaveRadiationAsync(Coordinates coordinates, CancellationToken cancellationToken)
    {
        var hourly = new HourlyShortwaveRadiation(
            [new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)],
            [42.0]);
        return Task.FromResult(hourly);
    }
}
