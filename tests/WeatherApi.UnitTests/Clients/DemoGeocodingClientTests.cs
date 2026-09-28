using Moq;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Infrastructure.Clients;
using Xunit;

namespace WeatherApi.UnitTests.Clients;

public sealed class DemoGeocodingClientTests
{
    private readonly Mock<IGeocodingClient> _inner = new();
    private readonly Mock<IDemoModeContext> _demoMode = new();

    private DemoGeocodingClient CreateSut() => new(_inner.Object, _demoMode.Object);

    [Fact]
    public async Task GeocodeAsync_WhenDemoDisabled_DelegatesToInner()
    {
        _demoMode.Setup(d => d.IsEnabled).Returns(false);
        var coordinates = new Coordinates(44.12, 4.08);
        _inner.Setup(c => c.GeocodeAsync("Alès", It.IsAny<CancellationToken>())).ReturnsAsync(coordinates);

        var result = await CreateSut().GeocodeAsync("Alès", CancellationToken.None);

        Assert.Equal(coordinates, result);
    }

    [Fact]
    public async Task GeocodeAsync_WhenDemoEnabled_NeverCallsInner()
    {
        _demoMode.Setup(d => d.IsEnabled).Returns(true);

        await CreateSut().GeocodeAsync("this-address-does-not-exist", CancellationToken.None);

        _inner.Verify(c => c.GeocodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GeocodeAsync_WhenDemoEnabled_ReturnsSimulatedCoordinates()
    {
        _demoMode.Setup(d => d.IsEnabled).Returns(true);

        var result = await CreateSut().GeocodeAsync("Alès", CancellationToken.None);

        Assert.NotNull(result);
        Assert.InRange(result!.Latitude, 41, 51);
        Assert.InRange(result.Longitude, -5, 9);
    }

    [Fact]
    public async Task GeocodeAsync_WhenDemoEnabled_IsDeterministicForTheSameAddress()
    {
        _demoMode.Setup(d => d.IsEnabled).Returns(true);
        var sut = CreateSut();

        var first = await sut.GeocodeAsync("Alès", CancellationToken.None);
        var second = await sut.GeocodeAsync("Alès", CancellationToken.None);

        Assert.Equal(first, second);
    }
}
