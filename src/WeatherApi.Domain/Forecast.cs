namespace WeatherApi.Domain;

public sealed record Forecast(string Address, Coordinates Coordinates, HourlyTemperature Hourly);
