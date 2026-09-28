using System.Text.Json.Serialization;

namespace WeatherApi.Infrastructure.Clients.Models;

/// <summary>
/// Typed model of MET Norway's locationforecast/2.0/compact response.
/// Only the fields the application needs are mapped.
/// </summary>
public sealed class MetNorwayForecastResponse
{
    [JsonPropertyName("properties")]
    public MetNorwayProperties? Properties { get; set; }
}

public sealed class MetNorwayProperties
{
    [JsonPropertyName("timeseries")]
    public List<MetNorwayTimeseriesEntry> Timeseries { get; set; } = [];
}

public sealed class MetNorwayTimeseriesEntry
{
    [JsonPropertyName("time")]
    public DateTimeOffset Time { get; set; }

    [JsonPropertyName("data")]
    public MetNorwayData? Data { get; set; }
}

public sealed class MetNorwayData
{
    [JsonPropertyName("instant")]
    public MetNorwayInstant? Instant { get; set; }
}

public sealed class MetNorwayInstant
{
    [JsonPropertyName("details")]
    public MetNorwayDetails? Details { get; set; }
}

public sealed class MetNorwayDetails
{
    [JsonPropertyName("air_temperature")]
    public double? AirTemperature { get; set; }
}
