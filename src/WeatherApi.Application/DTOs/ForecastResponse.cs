namespace WeatherApi.Application.DTOs;

/// <summary>
/// Public shape returned by the API. Decouples the wire contract from the Domain model.
/// Fixed regardless of the active geocoding/weather provider or demo mode — only the
/// content changes, never the shape (see TP3 "format de sortie unifié").
/// </summary>
public sealed record ForecastResponse(
    string Address,
    double Latitude,
    double Longitude,
    IReadOnlyList<HourlyForecastPoint> Hourly);

public sealed record HourlyForecastPoint(DateTimeOffset Time, double? TemperatureCelsius);
