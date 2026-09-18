using System.Net;
using WeatherApi.Domain.Exceptions;
using Xunit;

namespace WeatherApi.UnitTests.Contracts;

/// <summary>
/// A single contract test suite, run against every IWeatherClient implementation
/// (Open-Meteo, MET Norway) via a stub HttpMessageHandler — never a real network
/// call. Only Domain types (HourlyShortwaveRadiation, WeatherServiceException) are
/// touched here; no provider-specific model is ever referenced.
///
/// IWeatherClient takes Coordinates rather than a free-form address, so "adresse
/// introuvable" and "caractères accentués" don't translate directly: this suite
/// keeps the three scenarios that do apply (valid coordinates, no forecast data
/// available, malformed/empty response).
/// </summary>
public sealed class WeatherClientContractTests
{
    [Theory]
    [MemberData(nameof(WeatherProviderCase.All), MemberType = typeof(WeatherProviderCase))]
    public async Task GetHourlyShortwaveRadiationAsync_WithValidCoordinates_ReturnsHourlySeries(WeatherProviderCase provider)
    {
        var httpClient = StubHttpMessageHandler.CreateClient(HttpStatusCode.OK, provider.ValidCoordinatesResponseBody, out _);
        var client = provider.CreateClient(httpClient);

        var result = await client.GetHourlyShortwaveRadiationAsync(new(44.12, 4.08), CancellationToken.None);

        Assert.Equal(provider.ExpectedHourCount, result.Time.Count);
        Assert.Equal(provider.ExpectedHourCount, result.ShortwaveRadiation.Count);
    }

    [Theory]
    [MemberData(nameof(WeatherProviderCase.All), MemberType = typeof(WeatherProviderCase))]
    public async Task GetHourlyShortwaveRadiationAsync_WithNoForecastData_ThrowsWeatherServiceException(WeatherProviderCase provider)
    {
        var httpClient = StubHttpMessageHandler.CreateClient(HttpStatusCode.OK, provider.NoForecastResponseBody, out _);
        var client = provider.CreateClient(httpClient);

        await Assert.ThrowsAsync<WeatherServiceException>(
            () => client.GetHourlyShortwaveRadiationAsync(new(44.12, 4.08), CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(WeatherProviderCase.All), MemberType = typeof(WeatherProviderCase))]
    public async Task GetHourlyShortwaveRadiationAsync_WithEmptyHttpResponse_ThrowsWeatherServiceException(WeatherProviderCase provider)
    {
        var httpClient = StubHttpMessageHandler.CreateClient(HttpStatusCode.OK, string.Empty, out _);
        var client = provider.CreateClient(httpClient);

        await Assert.ThrowsAsync<WeatherServiceException>(
            () => client.GetHourlyShortwaveRadiationAsync(new(44.12, 4.08), CancellationToken.None));
    }
}
