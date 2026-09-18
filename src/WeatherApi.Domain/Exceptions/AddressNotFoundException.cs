namespace WeatherApi.Domain.Exceptions;

public sealed class AddressNotFoundException(string address)
    : Exception($"No location found for address '{address}'.")
{
    public string Address { get; } = address;
}
