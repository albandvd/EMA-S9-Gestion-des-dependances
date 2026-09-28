using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;

namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Decorates an IWeatherClient: when demo mode is active it never calls the
/// real provider and instead returns a deterministic simulated 24h series.
/// </summary>
public sealed class DemoWeatherClient(IWeatherClient inner, IDemoModeContext demoMode) : IWeatherClient
{
    private const int HourCount = 24;

    public Task<HourlyTemperature> GetHourlyTemperatureAsync(Coordinates coordinates, CancellationToken cancellationToken)
    {
        if (!demoMode.IsEnabled)
        {
            return inner.GetHourlyTemperatureAsync(coordinates, cancellationToken);
        }

        var start = DateTimeOffset.UtcNow.Date;
        var time = new List<DateTimeOffset>(HourCount);
        var temperature = new List<double?>(HourCount);
        var baseline = 15 + coordinates.Latitude / 10;

        for (var hour = 0; hour < HourCount; hour++)
        {
            time.Add(start.AddHours(hour));
            temperature.Add(Math.Round(baseline + 8 * Math.Sin((hour - 6) / 24.0 * 2 * Math.PI), 1));
        }

        return Task.FromResult(new HourlyTemperature(time, temperature));
    }
}
