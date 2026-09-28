namespace WeatherApi.Application.Interfaces;

/// <summary>
/// Tells whether the current request opted into demo mode (?demo=true), without
/// coupling the Application/Infrastructure layers to HTTP concerns. Implemented
/// in the Api layer, where the request is available.
/// </summary>
public interface IDemoModeContext
{
    bool IsEnabled { get; }
}
