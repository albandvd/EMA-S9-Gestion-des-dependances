namespace WeatherApi.Domain.Exceptions;

public sealed class GeocodingServiceException(string message, Exception? innerException = null)
    : Exception(message, innerException);
