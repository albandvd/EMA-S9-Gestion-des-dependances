namespace WeatherApi.Infrastructure.Configuration;

public sealed class BanOptions
{
    public const string SectionName = "Ban";

    public required string BaseUrl { get; set; }
}
