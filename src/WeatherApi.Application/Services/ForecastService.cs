using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain.Exceptions;

namespace WeatherApi.Application.Services;

/// <summary>
/// Orchestrates geocoding + weather lookup. Depends only on abstractions,
/// injected through the constructor (IoC) — no direct HTTP or external API knowledge.
/// </summary>
public sealed class ForecastService(IGeocodingClient geocodingClient, IWeatherClient weatherClient) : IForecastService
{
    public async Task<ForecastResponse> GetForecastAsync(string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address must not be empty.", nameof(address));
        }

        var coordinates = await geocodingClient.GeocodeAsync(address, cancellationToken);
        if (coordinates is null)
        {
            throw new AddressNotFoundException(address);
        }

        var hourly = await weatherClient.GetHourlyShortwaveRadiationAsync(coordinates, cancellationToken);

        return new ForecastResponse(
            address,
            coordinates.Latitude,
            coordinates.Longitude,
            new HourlyShortwaveRadiationResponse(hourly.Time, hourly.ShortwaveRadiation));
    }
}
