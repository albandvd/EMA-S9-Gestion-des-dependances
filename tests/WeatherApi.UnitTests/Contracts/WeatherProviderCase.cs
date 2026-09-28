using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WeatherApi.Application.Interfaces;
using WeatherApi.Infrastructure.Clients;
using WeatherApi.Infrastructure.Configuration;

namespace WeatherApi.UnitTests.Contracts;

/// <summary>
/// One entry per IWeatherClient implementation: how to build it, and the exact
/// wire-format bodies its own provider would send back for each contract scenario.
/// </summary>
public sealed record WeatherProviderCase(
    string Name,
    Func<HttpClient, IWeatherClient> CreateClient,
    string ValidCoordinatesResponseBody,
    int ExpectedHourCount,
    string NoForecastResponseBody)
{
    public override string ToString() => Name;

    public static readonly WeatherProviderCase OpenMeteo = new(
        "OpenMeteo",
        http => new OpenMeteoWeatherClient(
            http,
            Options.Create(new OpenMeteoOptions { BaseUrl = "https://stub.invalid/", HourlyParameters = "temperature_2m" }),
            NullLogger<OpenMeteoWeatherClient>.Instance),
        ValidCoordinatesResponseBody: """{"latitude":44.12,"longitude":4.08,"hourly":{"time":["2024-01-01T00:00:00Z","2024-01-01T01:00:00Z"],"temperature_2m":[0,12.5]}}""",
        ExpectedHourCount: 2,
        NoForecastResponseBody: """{"latitude":44.12,"longitude":4.08,"hourly":{"time":[],"temperature_2m":[]}}""");

    public static readonly WeatherProviderCase MetNorway = new(
        "MetNorway",
        http => new MetNorwayWeatherClient(http, NullLogger<MetNorwayWeatherClient>.Instance),
        ValidCoordinatesResponseBody: """{"properties":{"timeseries":[{"time":"2024-01-01T00:00:00Z","data":{"instant":{"details":{"air_temperature":0}}}},{"time":"2024-01-01T01:00:00Z","data":{"instant":{"details":{"air_temperature":12.5}}}}]}}""",
        ExpectedHourCount: 2,
        NoForecastResponseBody: """{"properties":{"timeseries":[]}}""");

    public static IEnumerable<object[]> All()
    {
        yield return [OpenMeteo];
        yield return [MetNorway];
    }
}
