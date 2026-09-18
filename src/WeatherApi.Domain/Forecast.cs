namespace WeatherApi.Domain;

public sealed record Forecast(string Address, Coordinates Coordinates, HourlyShortwaveRadiation Hourly);
