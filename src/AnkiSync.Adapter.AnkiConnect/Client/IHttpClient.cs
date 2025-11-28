using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnkiSync.Adapter.AnkiConnect.Client;

/// <summary>
/// Interface for HTTP client operations used by AnkiConnect
/// </summary>
public interface IHttpClient
{
    /// <summary>
    /// Sends a POST request to the specified URI with the given content
    /// </summary>
    Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the response content as JSON and deserializes it to the specified type
    /// </summary>
    Task<T?> ReadFromJsonAsync<T>(HttpContent content, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the response content as a string
    /// </summary>
    Task<string> ReadAsStringAsync(HttpContent content, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of IHttpClient using System.Net.Http.HttpClient
/// </summary>
public class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _httpClient;

    public HttpClientWrapper(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
    {
        return _httpClient.PostAsync(requestUri, content, cancellationToken);
    }

    public Task<T?> ReadFromJsonAsync<T>(HttpContent content, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        return content.ReadFromJsonAsync<T>(options, cancellationToken);
    }

    public Task<string> ReadAsStringAsync(HttpContent content, CancellationToken cancellationToken = default)
    {
        return content.ReadAsStringAsync(cancellationToken);
    }
}