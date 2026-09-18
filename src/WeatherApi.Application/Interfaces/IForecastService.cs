using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Interfaces;

public interface IForecastService
{
    /// <summary>
    /// Resolves an address to coordinates and returns the corresponding shortwave radiation forecast.
    /// </summary>
    /// <exception cref="ArgumentException">The address is null, empty or whitespace.</exception>
    /// <exception cref="Domain.Exceptions.AddressNotFoundException">No location matches the address.</exception>
    /// <exception cref="Domain.Exceptions.GeocodingServiceException">The geocoding provider failed.</exception>
    /// <exception cref="Domain.Exceptions.WeatherServiceException">The weather provider failed.</exception>
    Task<ForecastResponse> GetForecastAsync(string address, CancellationToken cancellationToken);
}
