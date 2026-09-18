using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;
using WeatherApi.Infrastructure.Clients.Models;

namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Geocoding client backed by the Nominatim (OpenStreetMap) HTTP API.
/// The named HttpClient is configured (base address, User-Agent) in DI registration.
/// </summary>
public sealed class NominatimGeocodingClient(
    HttpClient httpClient,
    ILogger<NominatimGeocodingClient> logger) : IGeocodingClient
{
    public async Task<Coordinates?> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        var requestUri = $"search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

        List<NominatimResult>? results;
        try
        {
            results = await httpClient.GetFromJsonAsync<List<NominatimResult>>(requestUri, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Nominatim request failed for address {Address}", address);
            throw new GeocodingServiceException("The geocoding service is unavailable.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Nominatim request timed out for address {Address}", address);
            throw new GeocodingServiceException("The geocoding service timed out.", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            logger.LogError(ex, "Nominatim returned an unparsable response for address {Address}", address);
            throw new GeocodingServiceException("The geocoding service returned an invalid response.", ex);
        }

        var first = results?.FirstOrDefault();
        if (first is null)
        {
            return null;
        }

        if (!double.TryParse(first.Lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude) ||
            !double.TryParse(first.Lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
        {
            throw new GeocodingServiceException("The geocoding service returned invalid coordinates.");
        }

        return new Coordinates(latitude, longitude);
    }
}
