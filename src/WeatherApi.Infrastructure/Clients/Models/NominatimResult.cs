using System.Text.Json.Serialization;

namespace WeatherApi.Infrastructure.Clients.Models;

/// <summary>
/// Typed model of a single element from Nominatim's /search response.
/// Only the fields the application needs are mapped.
/// </summary>
public sealed class NominatimResult
{
    [JsonPropertyName("lat")]
    public required string Lat { get; set; }

    [JsonPropertyName("lon")]
    public required string Lon { get; set; }

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
}
