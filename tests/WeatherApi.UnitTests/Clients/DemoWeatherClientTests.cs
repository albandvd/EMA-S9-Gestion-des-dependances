using Moq;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Infrastructure.Clients;
using Xunit;

namespace WeatherApi.UnitTests.Clients;

public sealed class DemoWeatherClientTests
{
    private readonly Mock<IWeatherClient> _inner = new();
    private readonly Mock<IDemoModeContext> _demoMode = new();
    private static readonly Coordinates Coordinates = new(44.12, 4.08);

    private DemoWeatherClient CreateSut() => new(_inner.Object, _demoMode.Object);

    [Fact]
    public async Task GetHourlyTemperatureAsync_WhenDemoDisabled_DelegatesToInner()
    {
        _demoMode.Setup(d => d.IsEnabled).Returns(false);
        var hourly = new HourlyTemperature([DateTimeOffset.UtcNow], [24.3]);
        _inner.Setup(c => c.GetHourlyTemperatureAsync(Coordinates, It.IsAny<CancellationToken>())).ReturnsAsync(hourly);

        var result = await CreateSut().GetHourlyTemperatureAsync(Coordinates, CancellationToken.None);

        Assert.Same(hourly, result);
    }

    [Fact]
    public async Task GetHourlyTemperatureAsync_WhenDemoEnabled_NeverCallsInnerAndReturnsASeries()
    {
        _demoMode.Setup(d => d.IsEnabled).Returns(true);

        var result = await CreateSut().GetHourlyTemperatureAsync(Coordinates, CancellationToken.None);

        _inner.Verify(c => c.GetHourlyTemperatureAsync(It.IsAny<Coordinates>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.NotEmpty(result.Time);
        Assert.Equal(result.Time.Count, result.TemperatureCelsius.Count);
    }
}
