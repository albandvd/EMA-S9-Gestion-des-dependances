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

        var hourly = await weatherClient.GetHourlyTemperatureAsync(coordinates, cancellationToken);

        var points = new List<HourlyForecastPoint>(hourly.Time.Count);
        for (var i = 0; i < hourly.Time.Count; i++)
        {
            points.Add(new HourlyForecastPoint(hourly.Time[i], hourly.TemperatureCelsius[i]));
        }

        return new ForecastResponse(address, coordinates.Latitude, coordinates.Longitude, points);
    }
}
