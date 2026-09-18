using Microsoft.Extensions.Logging.Abstractions;
using WeatherApi.Application.Interfaces;
using WeatherApi.Domain;
using WeatherApi.Infrastructure.Clients;

namespace WeatherApi.UnitTests.Contracts;

/// <summary>
/// One entry per IGeocodingClient implementation: how to build it, and the exact
/// wire-format bodies its own provider would send back for each contract scenario.
/// The address used throughout is "Alès" so the same fixture also covers the
/// accented-characters scenario.
/// </summary>
public sealed record GeocodingProviderCase(
    string Name,
    Func<HttpClient, IGeocodingClient> CreateClient,
    string ValidAddressResponseBody,
    string NotFoundResponseBody,
    Coordinates ExpectedCoordinates)
{
    public override string ToString() => Name;

    public static readonly GeocodingProviderCase Nominatim = new(
        "Nominatim",
        http => new NominatimGeocodingClient(http, NullLogger<NominatimGeocodingClient>.Instance),
        ValidAddressResponseBody: """[{"lat":"44.1253665","lon":"4.0852818","display_name":"Alès, Gard, Occitanie, France métropolitaine"}]""",
        NotFoundResponseBody: "[]",
        ExpectedCoordinates: new Coordinates(44.1253665, 4.0852818));

    public static readonly GeocodingProviderCase Ban = new(
        "Ban",
        http => new BanGeocodingClient(http, NullLogger<BanGeocodingClient>.Instance),
        ValidAddressResponseBody: """{"type":"FeatureCollection","features":[{"type":"Feature","geometry":{"type":"Point","coordinates":[4.0852818,44.1253665]},"properties":{"label":"Alès"}}]}""",
        NotFoundResponseBody: """{"type":"FeatureCollection","features":[]}""",
        ExpectedCoordinates: new Coordinates(44.1253665, 4.0852818));

    public static IEnumerable<object[]> All()
    {
        yield return [Nominatim];
        yield return [Ban];
    }
}
