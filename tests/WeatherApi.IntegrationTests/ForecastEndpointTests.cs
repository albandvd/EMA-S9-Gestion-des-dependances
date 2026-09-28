using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Application.DTOs;
using WeatherApi.IntegrationTests.Fakes;

namespace WeatherApi.IntegrationTests;

public sealed class ForecastEndpointTests(WeatherApiFactory factory) : IClassFixture<WeatherApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_WithKnownAddress_Returns200WithForecast()
    {
        var response = await _client.GetAsync("/forecast?address=Al%C3%A8s");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ForecastResponse>();
        Assert.NotNull(body);
        Assert.Equal("Alès", body!.Address);
        Assert.Equal(44.1253665, body.Latitude);
        Assert.Equal(4.0852818, body.Longitude);
        var point = Assert.Single(body.Hourly);
        Assert.NotNull(point.TemperatureCelsius);
    }

    [Fact]
    public async Task Get_WithoutAddress_Returns400()
    {
        var response = await _client.GetAsync("/forecast?address=");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithUnknownAddress_Returns404()
    {
        var response = await _client.GetAsync("/forecast?address=this-address-does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithDemoTrue_ReturnsSimulatedDataEvenForAnUnknownAddress()
    {
        // The fake geocoding client returns null for this address (would be a
        // 404 without demo mode) — demo mode must short-circuit before it.
        var response = await _client.GetAsync("/forecast?address=this-address-does-not-exist&demo=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ForecastResponse>();
        Assert.NotNull(body);
        Assert.Equal("this-address-does-not-exist", body!.Address);
        Assert.NotEmpty(body.Hourly);
    }

    [Fact]
    public async Task Get_CalledTwiceForSameAddress_OnlyGeocodesOnce()
    {
        var geocodingClient = factory.Services.GetRequiredService<FakeGeocodingClient>();
        var callsBefore = geocodingClient.CallCount;

        await _client.GetAsync("/forecast?address=N%C3%AEmes");
        await _client.GetAsync("/forecast?address=N%C3%AEmes");

        Assert.Equal(callsBefore + 1, geocodingClient.CallCount);
    }
}
