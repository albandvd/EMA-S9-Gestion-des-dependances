namespace WeatherApi.Domain;

public sealed record HourlyShortwaveRadiation(IReadOnlyList<DateTimeOffset> Time, IReadOnlyList<double?> ShortwaveRadiation);
