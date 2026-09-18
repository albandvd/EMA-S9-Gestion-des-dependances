using WeatherApi.Domain;

namespace WeatherApi.Application.Interfaces;

/// <summary>
/// Abstraction over a weather forecast provider. Implementations live in Infrastructure
/// and must never leak HTTP concerns to callers.
/// </summary>
public interface IWeatherClient
{
    Task<HourlyShortwaveRadiation> GetHourlyShortwaveRadiationAsync(Coordinates coordinates, CancellationToken cancellationToken);
}
