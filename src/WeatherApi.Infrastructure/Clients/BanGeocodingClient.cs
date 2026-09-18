using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;
using WeatherApi.Infrastructure.Clients.Models;

namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Geocoding client backed by the Base Adresse Nationale (BAN) HTTP API.
/// The named HttpClient is configured (base address) in DI registration.
/// </summary>
public sealed class BanGeocodingClient(HttpClient httpClient, ILogger<BanGeocodingClient> logger) : IGeocodingClient
{
    public async Task<Coordinates?> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        var requestUri = $"search/?q={Uri.EscapeDataString(address)}&limit=1";

        BanFeatureCollection? result;
        try
        {
            result = await httpClient.GetFromJsonAsync<BanFeatureCollection>(requestUri, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "BAN request failed for address {Address}", address);
            throw new GeocodingServiceException("The geocoding service is unavailable.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "BAN request timed out for address {Address}", address);
            throw new GeocodingServiceException("The geocoding service timed out.", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            logger.LogError(ex, "BAN returned an unparsable response for address {Address}", address);
            throw new GeocodingServiceException("The geocoding service returned an invalid response.", ex);
        }

        var coordinates = result?.Features.FirstOrDefault()?.Geometry?.Coordinates;
        if (coordinates is not { Length: >= 2 })
        {
            return null;
        }

        // GeoJSON order is [longitude, latitude].
        return new Coordinates(coordinates[1], coordinates[0]);
    }
}
