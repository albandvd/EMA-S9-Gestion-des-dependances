using WeatherApi.Infrastructure.Configuration;
using Xunit;

namespace WeatherApi.UnitTests.Configuration;

public sealed class ProviderSelectorTests
{
    private static readonly string[] ValidValues = ["Nominatim", "Ban"];

    [Fact]
    public void Resolve_WithNoConfiguredValue_ReturnsDefault()
    {
        var result = ProviderSelector.Resolve(null, "Nominatim", ValidValues);

        Assert.Equal("Nominatim", result);
    }

    [Theory]
    [InlineData("Ban")]
    [InlineData("ban")]
    [InlineData(" BAN ")]
    public void Resolve_WithKnownValueAnyCase_ReturnsCanonicalName(string configuredValue)
    {
        var result = ProviderSelector.Resolve(configuredValue, "Nominatim", ValidValues);

        Assert.Equal("Ban", result);
    }

    [Fact]
    public void Resolve_WithUnknownValue_ThrowsInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => ProviderSelector.Resolve("MetNorway", "Nominatim", ValidValues));

        Assert.Contains("MetNorway", exception.Message);
        Assert.Contains("Nominatim", exception.Message);
        Assert.Contains("Ban", exception.Message);
    }
}
