namespace WeatherApi.Infrastructure.Configuration;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public TimeSpan GeocodingTtl { get; set; } = TimeSpan.FromMinutes(15);
}
