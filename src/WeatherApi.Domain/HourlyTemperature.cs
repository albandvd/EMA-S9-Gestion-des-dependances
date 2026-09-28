namespace WeatherApi.Domain;

public sealed record HourlyTemperature(IReadOnlyList<DateTimeOffset> Time, IReadOnlyList<double?> TemperatureCelsius);
