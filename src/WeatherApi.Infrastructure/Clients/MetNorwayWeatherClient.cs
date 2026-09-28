using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;
using WeatherApi.Infrastructure.Clients.Models;

namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Weather client backed by MET Norway's Locationforecast 2.0 (compact) HTTP API.
/// The named HttpClient is configured (base address, User-Agent) in DI registration.
/// </summary>
public sealed class MetNorwayWeatherClient(HttpClient httpClient, ILogger<MetNorwayWeatherClient> logger) : IWeatherClient
{
    public async Task<HourlyTemperature> GetHourlyTemperatureAsync(Coordinates coordinates, CancellationToken cancellationToken)
    {
        var lat = coordinates.Latitude.ToString(CultureInfo.InvariantCulture);
        var lon = coordinates.Longitude.ToString(CultureInfo.InvariantCulture);
        var requestUri = $"weatherapi/locationforecast/2.0/compact?lat={lat}&lon={lon}";

        MetNorwayForecastResponse? response;
        try
        {
            response = await httpClient.GetFromJsonAsync<MetNorwayForecastResponse>(requestUri, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "MET Norway request failed for coordinates {Latitude},{Longitude}", lat, lon);
            throw new WeatherServiceException("The weather service is unavailable.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "MET Norway request timed out for coordinates {Latitude},{Longitude}", lat, lon);
            throw new WeatherServiceException("The weather service timed out.", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            logger.LogError(ex, "MET Norway returned an unparsable response for coordinates {Latitude},{Longitude}", lat, lon);
            throw new WeatherServiceException("The weather service returned an invalid response.", ex);
        }

        var timeseries = response?.Properties?.Timeseries;
        if (timeseries is not { Count: > 0 })
        {
            throw new WeatherServiceException("The weather service returned no hourly data.");
        }

        var time = timeseries.Select(entry => entry.Time).ToList();
        var temperature = timeseries.Select(entry => entry.Data?.Instant?.Details?.AirTemperature).ToList();

        return new HourlyTemperature(time, temperature);
    }
}
