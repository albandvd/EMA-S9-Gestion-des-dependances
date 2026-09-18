namespace WeatherApi.Application.DTOs;

/// <summary>
/// Public shape returned by the API. Decouples the wire contract from the Domain model.
/// </summary>
public sealed record ForecastResponse(
    string Address,
    double Latitude,
    double Longitude,
    HourlyShortwaveRadiationResponse Hourly);

public sealed record HourlyShortwaveRadiationResponse(
    IReadOnlyList<DateTimeOffset> Time,
    IReadOnlyList<double?> ShortwaveRadiation);
