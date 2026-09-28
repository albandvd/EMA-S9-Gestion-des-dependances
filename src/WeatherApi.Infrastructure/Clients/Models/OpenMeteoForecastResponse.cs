using System.Text.Json.Serialization;

namespace WeatherApi.Infrastructure.Clients.Models;

/// <summary>
/// Typed model of Open-Meteo's /v1/forecast response.
/// Only the fields the application needs are mapped.
/// </summary>
public sealed class OpenMeteoForecastResponse
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("hourly")]
    public OpenMeteoHourly? Hourly { get; set; }
}

public sealed class OpenMeteoHourly
{
    [JsonPropertyName("time")]
    public List<DateTimeOffset> Time { get; set; } = [];

    [JsonPropertyName("temperature_2m")]
    public List<double?> Temperature { get; set; } = [];
}
