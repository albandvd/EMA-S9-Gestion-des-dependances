using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeatherApi.Application.Interfaces;
using WeatherApi.Infrastructure.Clients;
using WeatherApi.IntegrationTests.Fakes;

namespace WeatherApi.IntegrationTests;

/// <summary>
/// Boots the real ASP.NET Core pipeline (routing, DI, middleware, JSON) while
/// swapping the external HTTP clients for deterministic fakes. The fakes are
/// still wired through the real demo decorators (from AddInfrastructure) so
/// this factory also exercises TP3's demo mode end to end.
/// </summary>
public sealed class WeatherApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGeocodingClient>();
            services.AddSingleton<FakeGeocodingClient>();
            services.AddScoped<IGeocodingClient>(sp => new DemoGeocodingClient(
                sp.GetRequiredService<FakeGeocodingClient>(),
                sp.GetRequiredService<IDemoModeContext>()));

            services.RemoveAll<IWeatherClient>();
            services.AddSingleton<FakeWeatherClient>();
            services.AddScoped<IWeatherClient>(sp => new DemoWeatherClient(
                sp.GetRequiredService<FakeWeatherClient>(),
                sp.GetRequiredService<IDemoModeContext>()));
        });
    }
}
