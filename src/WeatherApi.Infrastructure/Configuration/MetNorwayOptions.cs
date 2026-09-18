namespace WeatherApi.Infrastructure.Configuration;

public sealed class MetNorwayOptions
{
    public const string SectionName = "MetNorway";

    public required string BaseUrl { get; set; }

    /// <summary>
    /// MET Norway rejects requests with a missing or default HTTP client User-Agent (403).
    /// Must identify the application and a contact (email or website).
    /// </summary>
    public required string UserAgent { get; set; }
}
