namespace WeatherApi.Infrastructure.Configuration;

public sealed class NominatimOptions
{
    public const string SectionName = "Nominatim";

    public required string BaseUrl { get; set; }
    public required string UserAgent { get; set; }
}
