using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Application.Interfaces;
using WeatherApi.Infrastructure.Clients;
using WeatherApi.Infrastructure.Configuration;

namespace WeatherApi.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer: registers external service
/// implementations behind their Application-layer abstractions, and configures
/// each named HttpClient via IHttpClientFactory.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<NominatimOptions>()
            .Bind(configuration.GetSection(NominatimOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<OpenMeteoOptions>()
            .Bind(configuration.GetSection(OpenMeteoOptions.SectionName))
            .ValidateOnStart();

        services.AddHttpClient<IGeocodingClient, NominatimGeocodingClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<NominatimOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        });

        services.AddHttpClient<IWeatherClient, OpenMeteoWeatherClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<OpenMeteoOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}
