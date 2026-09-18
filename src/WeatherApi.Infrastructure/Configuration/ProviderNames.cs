namespace WeatherApi.Infrastructure.Configuration;

/// <summary>
/// Canonical keys used both for keyed DI registration and for the
/// "Geocoding:Provider" configuration value.
/// </summary>
public static class GeocodingProviders
{
    public const string Nominatim = "Nominatim";
    public const string Ban = "Ban";

    public static readonly IReadOnlyList<string> All = [Nominatim, Ban];
}

/// <summary>
/// Canonical keys used both for keyed DI registration and for the
/// "Weather:Provider" configuration value.
/// </summary>
public static class WeatherProviders
{
    public const string OpenMeteo = "OpenMeteo";
    public const string MetNorway = "MetNorway";

    public static readonly IReadOnlyList<string> All = [OpenMeteo, MetNorway];
}
