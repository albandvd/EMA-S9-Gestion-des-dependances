using System.Reflection;
using WeatherApi.Application.Interfaces;
using Xunit;

namespace WeatherApi.UnitTests.Contracts;

/// <summary>
/// Guards, mechanically rather than by convention, that IGeocodingClient and
/// IWeatherClient never expose a provider-specific type (NominatimResult,
/// BanFeatureCollection, OpenMeteoForecastResponse, MetNorwayForecastResponse, ...).
/// If a future change adds a method that returns or accepts an Infrastructure
/// type, this test fails immediately instead of the leak being caught by review.
/// </summary>
public sealed class AbstractionLeakageTests
{
    [Theory]
    [InlineData(typeof(IGeocodingClient))]
    [InlineData(typeof(IWeatherClient))]
    public void Contract_NeverExposesAnInfrastructureType(Type contractType)
    {
        foreach (var method in contractType.GetMethods())
        {
            foreach (var type in SignatureTypes(method))
            {
                Assert.False(
                    (type.Namespace ?? string.Empty).StartsWith("WeatherApi.Infrastructure", StringComparison.Ordinal),
                    $"{contractType.Name}.{method.Name} exposes Infrastructure type '{type.FullName}'.");
            }
        }
    }

    private static IEnumerable<Type> SignatureTypes(MethodInfo method)
    {
        yield return Unwrap(method.ReturnType);
        foreach (var parameter in method.GetParameters())
        {
            yield return Unwrap(parameter.ParameterType);
        }
    }

    private static Type Unwrap(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)
            ? type.GetGenericArguments()[0]
            : type;
}
