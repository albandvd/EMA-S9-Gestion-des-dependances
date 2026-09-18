using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;
using WeatherApi.Infrastructure.Clients.Models;
using WeatherApi.Infrastructure.Configuration;

namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Weather client backed by the Open-Meteo HTTP API.
/// The named HttpClient is configured (base address) in DI registration.
/// </summary>
public sealed class OpenMeteoWeatherClient(
    HttpClient httpClient,
    IOptions<OpenMeteoOptions> options,
    ILogger<OpenMeteoWeatherClient> logger) : IWeatherClient
{
    public async Task<HourlyShortwaveRadiation> GetHourlyShortwaveRadiationAsync(Coordinates coordinates, CancellationToken cancellationToken)
    {
        var lat = coordinates.Latitude.ToString(CultureInfo.InvariantCulture);
        var lon = coordinates.Longitude.ToString(CultureInfo.InvariantCulture);
        var requestUri = $"v1/forecast?latitude={lat}&longitude={lon}&hourly={options.Value.HourlyParameters}";

        OpenMeteoForecastResponse? response;
        try
        {
            response = await httpClient.GetFromJsonAsync<OpenMeteoForecastResponse>(requestUri, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Open-Meteo request failed for coordinates {Latitude},{Longitude}", lat, lon);
            throw new WeatherServiceException("The weather service is unavailable.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Open-Meteo request timed out for coordinates {Latitude},{Longitude}", lat, lon);
            throw new WeatherServiceException("The weather service timed out.", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            logger.LogError(ex, "Open-Meteo returned an unparsable response for coordinates {Latitude},{Longitude}", lat, lon);
            throw new WeatherServiceException("The weather service returned an invalid response.", ex);
        }

        if (response?.Hourly is null)
        {
            throw new WeatherServiceException("The weather service returned no hourly data.");
        }

        return new HourlyShortwaveRadiation(response.Hourly.Time, response.Hourly.ShortwaveRadiation);
    }
}
