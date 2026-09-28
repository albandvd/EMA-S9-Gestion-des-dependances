using Moq;
using WeatherApi.Application.Interfaces;
using WeatherApi.Application.Services;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;
using Xunit;

namespace WeatherApi.UnitTests.Services;

/// <summary>
/// Unit tests for ForecastService. All external dependencies are mocked —
/// no real network call is ever made.
/// </summary>
public sealed class ForecastServiceTests
{
    private readonly Mock<IGeocodingClient> _geocodingClient = new();
    private readonly Mock<IWeatherClient> _weatherClient = new();

    private ForecastService CreateSut() => new(_geocodingClient.Object, _weatherClient.Object);

    [Fact]
    public async Task GetForecastAsync_WithFoundAddress_ReturnsForecastResponse()
    {
        const string address = "Alès";
        var coordinates = new Coordinates(44.1253665, 4.0852818);
        var hourly = new HourlyTemperature(
            [new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)],
            [24.3]);

        _geocodingClient
            .Setup(c => c.GeocodeAsync(address, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coordinates);
        _weatherClient
            .Setup(c => c.GetHourlyTemperatureAsync(coordinates, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hourly);

        var sut = CreateSut();

        var result = await sut.GetForecastAsync(address, CancellationToken.None);

        Assert.Equal(address, result.Address);
        Assert.Equal(coordinates.Latitude, result.Latitude);
        Assert.Equal(coordinates.Longitude, result.Longitude);
        var point = Assert.Single(result.Hourly);
        Assert.Equal(hourly.Time[0], point.Time);
        Assert.Equal(hourly.TemperatureCelsius[0], point.TemperatureCelsius);
    }

    [Fact]
    public async Task GetForecastAsync_WithUnknownAddress_ThrowsAddressNotFoundException()
    {
        const string address = "does-not-exist-anywhere-12345";
        _geocodingClient
            .Setup(c => c.GeocodeAsync(address, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coordinates?)null);

        var sut = CreateSut();

        var exception = await Assert.ThrowsAsync<AddressNotFoundException>(
            () => sut.GetForecastAsync(address, CancellationToken.None));
        Assert.Equal(address, exception.Address);
        _weatherClient.Verify(
            c => c.GetHourlyTemperatureAsync(It.IsAny<Coordinates>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetForecastAsync_WhenGeocodingServiceFails_ThrowsGeocodingServiceException()
    {
        const string address = "Alès";
        _geocodingClient
            .Setup(c => c.GeocodeAsync(address, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new GeocodingServiceException("The geocoding service is unavailable."));

        var sut = CreateSut();

        await Assert.ThrowsAsync<GeocodingServiceException>(
            () => sut.GetForecastAsync(address, CancellationToken.None));
    }

    [Fact]
    public async Task GetForecastAsync_WhenWeatherServiceFails_ThrowsWeatherServiceException()
    {
        const string address = "Alès";
        var coordinates = new Coordinates(44.1253665, 4.0852818);
        _geocodingClient
            .Setup(c => c.GeocodeAsync(address, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coordinates);
        _weatherClient
            .Setup(c => c.GetHourlyTemperatureAsync(coordinates, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WeatherServiceException("The weather service is unavailable."));

        var sut = CreateSut();

        await Assert.ThrowsAsync<WeatherServiceException>(
            () => sut.GetForecastAsync(address, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetForecastAsync_WithEmptyAddress_ThrowsArgumentException(string? address)
    {
        var sut = CreateSut();

        await Assert.ThrowsAsync<ArgumentException>(
            () => sut.GetForecastAsync(address!, CancellationToken.None));

        _geocodingClient.Verify(
            c => c.GeocodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
