namespace WeatherApi.Infrastructure.Clients;

/// <summary>
/// Small deterministic helper shared by the demo-mode decorators: turns a string
/// into a stable value in [0, 1), independent of .NET's randomized string hashing.
/// </summary>
internal static class DemoData
{
    public static double HashToUnitInterval(string input, string salt)
    {
        unchecked
        {
            var hash = 17;
            foreach (var c in input.Trim().ToLowerInvariant() + salt)
            {
                hash = hash * 31 + c;
            }

            return (double)(Math.Abs(hash) % 10_000) / 10_000;
        }
    }
}
