using System.Net;
using WeatherApi.Domain.Exceptions;
using Xunit;

namespace WeatherApi.UnitTests.Contracts;

/// <summary>
/// A single contract test suite, run against every IGeocodingClient implementation
/// (Nominatim, BAN). Each adapter is fed the exact wire format its own provider
/// returns via a stub HttpMessageHandler — never a real network call — and every
/// assertion here only touches Domain types (Coordinates, GeocodingServiceException).
/// This file never references a provider-specific model (NominatimResult,
/// BanFeatureCollection, ...): if it compiled while doing so, that would itself be
/// proof of a leak.
/// </summary>
public sealed class GeocodingClientContractTests
{
    private const string Address = "Alès";

    [Theory]
    [MemberData(nameof(GeocodingProviderCase.All), MemberType = typeof(GeocodingProviderCase))]
    public async Task GeocodeAsync_WithValidAddress_ReturnsCoordinates(GeocodingProviderCase provider)
    {
        var httpClient = StubHttpMessageHandler.CreateClient(HttpStatusCode.OK, provider.ValidAddressResponseBody, out _);
        var client = provider.CreateClient(httpClient);

        var result = await client.GeocodeAsync(Address, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(provider.ExpectedCoordinates.Latitude, result!.Latitude, precision: 5);
        Assert.Equal(provider.ExpectedCoordinates.Longitude, result.Longitude, precision: 5);
    }

    [Theory]
    [MemberData(nameof(GeocodingProviderCase.All), MemberType = typeof(GeocodingProviderCase))]
    public async Task GeocodeAsync_WithUnknownAddress_ReturnsNull(GeocodingProviderCase provider)
    {
        var httpClient = StubHttpMessageHandler.CreateClient(HttpStatusCode.OK, provider.NotFoundResponseBody, out _);
        var client = provider.CreateClient(httpClient);

        var result = await client.GeocodeAsync("does-not-exist-anywhere-12345", CancellationToken.None);

        Assert.Null(result);
    }

    [Theory]
    [MemberData(nameof(GeocodingProviderCase.All), MemberType = typeof(GeocodingProviderCase))]
    public async Task GeocodeAsync_WithEmptyHttpResponse_ThrowsGeocodingServiceException(GeocodingProviderCase provider)
    {
        var httpClient = StubHttpMessageHandler.CreateClient(HttpStatusCode.OK, string.Empty, out _);
        var client = provider.CreateClient(httpClient);

        await Assert.ThrowsAsync<GeocodingServiceException>(() => client.GeocodeAsync(Address, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(GeocodingProviderCase.All), MemberType = typeof(GeocodingProviderCase))]
    public async Task GeocodeAsync_WithAccentedAddress_EncodesRequestAndDecodesResponse(GeocodingProviderCase provider)
    {
        var httpClient = StubHttpMessageHandler.CreateClient(HttpStatusCode.OK, provider.ValidAddressResponseBody, out var handler);
        var client = provider.CreateClient(httpClient);

        var result = await client.GeocodeAsync(Address, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(provider.ExpectedCoordinates.Latitude, result!.Latitude, precision: 5);
        Assert.Equal(provider.ExpectedCoordinates.Longitude, result.Longitude, precision: 5);

        // The accented character must reach the provider correctly percent-encoded,
        // and the accented text embedded in the stubbed response must not blow up
        // JSON/UTF-8 decoding.
        var sentQuery = Uri.UnescapeDataString(handler.LastRequest!.RequestUri!.Query);
        Assert.Contains(Address, sentQuery);
    }
}
