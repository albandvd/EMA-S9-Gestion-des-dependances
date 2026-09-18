using System.Net;
using System.Text;

namespace WeatherApi.UnitTests.Contracts;

/// <summary>
/// Fake transport that never touches the network: it hands back a canned HTTP
/// response for every request and records the last request it saw, so contract
/// tests can assert on both the outgoing request and the parsed result.
/// </summary>
public sealed class StubHttpMessageHandler(HttpStatusCode statusCode, string? responseBody) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        var response = new HttpResponseMessage(statusCode)
        {
            Content = responseBody is null ? null : new StringContent(responseBody, Encoding.UTF8, "application/json"),
        };
        return Task.FromResult(response);
    }

    public static HttpClient CreateClient(HttpStatusCode statusCode, string? responseBody, out StubHttpMessageHandler handler)
    {
        handler = new StubHttpMessageHandler(statusCode, responseBody);
        return new HttpClient(handler) { BaseAddress = new Uri("https://stub.invalid/") };
    }
}
