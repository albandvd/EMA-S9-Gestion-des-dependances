using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Infrastructure.Caching;
using WeatherApi.Infrastructure.Clients;
using WeatherApi.Infrastructure.Configuration;
using Xunit;

namespace WeatherApi.UnitTests.Clients;

public sealed class CachingGeocodingClientTests
{
    private readonly Mock<IGeocodingClient> _inner = new();

    private CachingGeocodingClient CreateSut() =>
        new(_inner.Object, new MemoryResponseCache(new MemoryCache(new MemoryCacheOptions())), Options.Create(new CacheOptions()));

    [Fact]
    public async Task GeocodeAsync_CalledTwiceForSameAddress_OnlyCallsInnerOnce()
    {
        var coordinates = new Coordinates(44.12, 4.08);
        _inner.Setup(c => c.GeocodeAsync("Alès", It.IsAny<CancellationToken>())).ReturnsAsync(coordinates);
        var sut = CreateSut();

        var first = await sut.GeocodeAsync("Alès", CancellationToken.None);
        var second = await sut.GeocodeAsync("Alès", CancellationToken.None);

        Assert.Equal(coordinates, first);
        Assert.Equal(coordinates, second);
        _inner.Verify(c => c.GeocodeAsync("Alès", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GeocodeAsync_CalledForDifferentAddresses_CallsInnerForEach()
    {
        _inner.Setup(c => c.GeocodeAsync("Alès", It.IsAny<CancellationToken>())).ReturnsAsync(new Coordinates(44.12, 4.08));
        _inner.Setup(c => c.GeocodeAsync("Nîmes", It.IsAny<CancellationToken>())).ReturnsAsync(new Coordinates(43.83, 4.36));
        var sut = CreateSut();

        await sut.GeocodeAsync("Alès", CancellationToken.None);
        await sut.GeocodeAsync("Nîmes", CancellationToken.None);

        _inner.Verify(c => c.GeocodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GeocodeAsync_CachesAnAddressNotFoundResultToo()
    {
        _inner.Setup(c => c.GeocodeAsync("unknown", It.IsAny<CancellationToken>())).ReturnsAsync((Coordinates?)null);
        var sut = CreateSut();

        var first = await sut.GeocodeAsync("unknown", CancellationToken.None);
        var second = await sut.GeocodeAsync("unknown", CancellationToken.None);

        Assert.Null(first);
        Assert.Null(second);
        _inner.Verify(c => c.GeocodeAsync("unknown", It.IsAny<CancellationToken>()), Times.Once);
    }
}
