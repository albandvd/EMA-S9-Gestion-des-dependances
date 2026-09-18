using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeatherApi.Application.Interfaces;
using WeatherApi.Infrastructure.Clients;
using WeatherApi.Infrastructure.Configuration;

namespace WeatherApi.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer: registers every external
/// provider adapter behind a keyed DI registration, then exposes the
/// Application-layer abstraction (IGeocodingClient/IWeatherClient) as whichever
/// provider "Geocoding:Provider"/"Weather:Provider" selects at startup — no
/// recompilation needed to switch provider, only a config/env var change.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<NominatimOptions>().Bind(configuration.GetSection(NominatimOptions.SectionName)).ValidateOnStart();
        services.AddOptions<OpenMeteoOptions>().Bind(configuration.GetSection(OpenMeteoOptions.SectionName)).ValidateOnStart();
        services.AddOptions<BanOptions>().Bind(configuration.GetSection(BanOptions.SectionName)).ValidateOnStart();
        services.AddOptions<MetNorwayOptions>().Bind(configuration.GetSection(MetNorwayOptions.SectionName)).ValidateOnStart();

        services.AddHttpClient<NominatimGeocodingClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<NominatimOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        });

        services.AddHttpClient<BanGeocodingClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<BanOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddHttpClient<OpenMeteoWeatherClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<OpenMeteoOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddHttpClient<MetNorwayWeatherClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<MetNorwayOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        });

        services.AddKeyedTransient<IGeocodingClient>(GeocodingProviders.Nominatim, (sp, _) => sp.GetRequiredService<NominatimGeocodingClient>());
        services.AddKeyedTransient<IGeocodingClient>(GeocodingProviders.Ban, (sp, _) => sp.GetRequiredService<BanGeocodingClient>());

        services.AddKeyedTransient<IWeatherClient>(WeatherProviders.OpenMeteo, (sp, _) => sp.GetRequiredService<OpenMeteoWeatherClient>());
        services.AddKeyedTransient<IWeatherClient>(WeatherProviders.MetNorway, (sp, _) => sp.GetRequiredService<MetNorwayWeatherClient>());

        var geocodingProvider = ProviderSelector.Resolve(
            configuration["Geocoding:Provider"], GeocodingProviders.Nominatim, GeocodingProviders.All);
        var weatherProvider = ProviderSelector.Resolve(
            configuration["Weather:Provider"], WeatherProviders.OpenMeteo, WeatherProviders.All);

        services.AddTransient<IGeocodingClient>(sp => sp.GetRequiredKeyedService<IGeocodingClient>(geocodingProvider));
        services.AddTransient<IWeatherClient>(sp => sp.GetRequiredKeyedService<IWeatherClient>(weatherProvider));

        return services;
    }
}
