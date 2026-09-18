using System.Text.Json.Serialization;

namespace WeatherApi.Infrastructure.Clients.Models;

/// <summary>
/// Typed model of the Base Adresse Nationale (BAN) /search response (GeoJSON).
/// Only the fields the application needs are mapped.
/// </summary>
public sealed class BanFeatureCollection
{
    [JsonPropertyName("features")]
    public List<BanFeature> Features { get; set; } = [];
}

public sealed class BanFeature
{
    [JsonPropertyName("geometry")]
    public BanGeometry? Geometry { get; set; }
}

public sealed class BanGeometry
{
    /// <summary>GeoJSON order: [longitude, latitude].</summary>
    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; set; } = [];
}
