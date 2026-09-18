namespace WeatherApi.Infrastructure.Configuration;

public sealed class OpenMeteoOptions
{
    public const string SectionName = "OpenMeteo";

    public required string BaseUrl { get; set; }
    public string HourlyParameters { get; set; } = "shortwave_radiation";
}
