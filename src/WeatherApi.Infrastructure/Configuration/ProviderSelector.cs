namespace WeatherApi.Infrastructure.Configuration;

/// <summary>
/// Resolves a configured provider name (from appsettings/env var) to one of the
/// canonical keys used for keyed DI registration, case-insensitively, and fails
/// fast at startup with a clear message if the value is unknown.
/// </summary>
public static class ProviderSelector
{
    public static string Resolve(string? configuredValue, string defaultValue, IReadOnlyCollection<string> validValues)
    {
        var value = string.IsNullOrWhiteSpace(configuredValue) ? defaultValue : configuredValue.Trim();
        var match = validValues.FirstOrDefault(v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase));

        return match ?? throw new InvalidOperationException(
            $"Unknown provider '{value}'. Valid values: {string.Join(", ", validValues)}.");
    }
}
