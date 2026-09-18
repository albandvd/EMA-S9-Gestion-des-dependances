namespace WeatherApi.Domain.Exceptions;

public sealed class WeatherServiceException(string message, Exception? innerException = null)
    : Exception(message, innerException);
