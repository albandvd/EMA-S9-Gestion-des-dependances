using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeatherApi.Application.Interfaces;
using WeatherApi.IntegrationTests.Fakes;

namespace WeatherApi.IntegrationTests;

/// <summary>
/// Boots the real ASP.NET Core pipeline (routing, DI, middleware, JSON) while
/// swapping the external HTTP clients for deterministic fakes.
/// </summary>
public sealed class WeatherApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGeocodingClient>();
            services.AddSingleton<IGeocodingClient, FakeGeocodingClient>();

            services.RemoveAll<IWeatherClient>();
            services.AddSingleton<IWeatherClient, FakeWeatherClient>();
        });
    }
}
