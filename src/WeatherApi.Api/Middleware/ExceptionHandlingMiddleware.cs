using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Domain.Exceptions;

namespace WeatherApi.Api.Middleware;

/// <summary>
/// Translates exceptions raised by the Application/Infrastructure layers into
/// ProblemDetails HTTP responses. Guarantees no exception detail or stack trace
/// is ever exposed to the client.
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Invalid request", ex.Message);
        }
        catch (AddressNotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Address not found", ex.Message);
        }
        catch (GeocodingServiceException ex)
        {
            logger.LogError(ex, "Geocoding service failure");
            await WriteProblemAsync(context, StatusCodes.Status502BadGateway, "Geocoding service unavailable",
                "The geocoding service is currently unavailable. Please try again later.");
        }
        catch (WeatherServiceException ex)
        {
            logger.LogError(ex, "Weather service failure");
            await WriteProblemAsync(context, StatusCodes.Status502BadGateway, "Weather service unavailable",
                "The weather service is currently unavailable. Please try again later.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Unexpected error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(problemDetails);
        await context.Response.Body.WriteAsync(bytes);
    }
}
