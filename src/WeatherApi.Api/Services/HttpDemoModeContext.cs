using Microsoft.AspNetCore.Http;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Api.Services;

/// <summary>
/// Reads the "demo" query parameter off the current request. This is the only
/// place that knows demo mode is driven by HTTP — everything downstream just
/// sees IDemoModeContext.
/// </summary>
public sealed class HttpDemoModeContext(IHttpContextAccessor httpContextAccessor) : IDemoModeContext
{
    public bool IsEnabled =>
        bool.TryParse(httpContextAccessor.HttpContext?.Request.Query["demo"].ToString(), out var value) && value;
}
