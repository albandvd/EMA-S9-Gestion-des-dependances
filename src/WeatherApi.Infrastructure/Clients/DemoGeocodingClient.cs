using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;

namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Decorates an IGeocodingClient: when demo mode is active it never calls the
/// real provider and instead returns deterministic simulated coordinates.
/// </summary>
public sealed class DemoGeocodingClient(IGeocodingClient inner, IDemoModeContext demoMode) : IGeocodingClient
{
    public Task<Coordinates?> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        if (!demoMode.IsEnabled)
        {
            return inner.GeocodeAsync(address, cancellationToken);
        }

        // Roughly the bounding box of metropolitan France, so different demo
        // addresses still land in plausible, distinct spots.
        var latitude = 41 + DemoData.HashToUnitInterval(address, "lat") * 10;
        var longitude = -5 + DemoData.HashToUnitInterval(address, "lon") * 14;

        return Task.FromResult<Coordinates?>(new Coordinates(Math.Round(latitude, 4), Math.Round(longitude, 4)));
    }
}
