using System.Net;
using System.Net.Http.Json;
using WeatherApi.Application.DTOs;

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
}
