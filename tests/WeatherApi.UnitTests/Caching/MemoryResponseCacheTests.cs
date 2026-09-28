using Microsoft.Extensions.Caching.Memory;
using WeatherApi.Infrastructure.Caching;
using Xunit;

namespace WeatherApi.UnitTests.Caching;

public sealed class MemoryResponseCacheTests
{
    [Fact]
    public async Task GetOrCreateAsync_CalledTwiceWithSameKey_OnlyInvokesFactoryOnce()
    {
        var cache = new MemoryResponseCache(new MemoryCache(new MemoryCacheOptions()));
        var callCount = 0;

        Task<int> Factory(CancellationToken ct)
        {
            callCount++;
            return Task.FromResult(42);
        }

        var first = await cache.GetOrCreateAsync("key", TimeSpan.FromMinutes(1), Factory, CancellationToken.None);
        var second = await cache.GetOrCreateAsync("key", TimeSpan.FromMinutes(1), Factory, CancellationToken.None);

        Assert.Equal(42, first);
        Assert.Equal(42, second);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task GetOrCreateAsync_CalledWithDifferentKeys_InvokesFactoryForEach()
    {
        var cache = new MemoryResponseCache(new MemoryCache(new MemoryCacheOptions()));
        var callCount = 0;

        Task<int> Factory(CancellationToken ct)
        {
            callCount++;
            return Task.FromResult(callCount);
        }

        await cache.GetOrCreateAsync("key-a", TimeSpan.FromMinutes(1), Factory, CancellationToken.None);
        await cache.GetOrCreateAsync("key-b", TimeSpan.FromMinutes(1), Factory, CancellationToken.None);

        Assert.Equal(2, callCount);
    }
}
