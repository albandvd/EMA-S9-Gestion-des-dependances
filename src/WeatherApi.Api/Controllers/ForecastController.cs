using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ForecastController(IForecastService forecastService) : ControllerBase
{
    /// <summary>
    /// Returns the shortwave radiation hourly forecast for the given address.
    /// </summary>
    /// <param name="address">A postal address or city name.</param>
    /// <response code="200">The forecast was retrieved successfully.</response>
    /// <response code="400">The address is missing or empty.</response>
    /// <response code="404">No location was found for the given address.</response>
    /// <response code="502">An external service (geocoding or weather) is unavailable.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ForecastResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<ForecastResponse>> Get([FromQuery] string? address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return Problem(
                title: "Invalid request",
                detail: "The 'address' query parameter is required and must not be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var forecast = await forecastService.GetForecastAsync(address, cancellationToken);
        return Ok(forecast);
    }
}
