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
}
