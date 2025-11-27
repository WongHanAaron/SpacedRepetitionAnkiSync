using AnkiSync.Adapter.AnkiConnect.Configuration;
using AnkiSync.Domain.Core.Exceptions;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnkiSync.Adapter.AnkiConnect.Client;

/// <summary>
/// HTTP client for communicating with Anki's AnkiConnect plugin
/// </summary>
public class AnkiConnectHttpClient : IAnkiConnectHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AnkiConnectOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public AnkiConnectHttpClient(AnkiConnectOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
            Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
        };

        // Configure JSON serialization to match AnkiConnect expectations
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Sends a request to AnkiConnect and returns the response
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <param name="action">The AnkiConnect action</param>
    /// <param name="parameters">The action parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response result</returns>
    public async Task<TResponse> InvokeAsync<TRequest, TResponse>(
        string action,
        TRequest parameters,
        CancellationToken cancellationToken = default)
    {
        var request = new AnkiConnectRequest<TRequest>
        {
            Action = action,
            Version = 6,
            Params = parameters
        };

        var response = await SendRequestAsync<AnkiConnectRequest<TRequest>, AnkiConnectResponse<TResponse>>(
            request, cancellationToken);

        if (response.Error != null)
        {
            throw new AnkiConnectionException(
                $"AnkiConnect error for action '{action}': {response.Error.Message}");
        }

        return response.Result!;
    }

    /// <summary>
    /// Sends a request without parameters
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <param name="action">The AnkiConnect action</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response result</returns>
    public async Task<TResponse> InvokeAsync<TResponse>(
        string action,
        CancellationToken cancellationToken = default)
    {
        return await InvokeAsync<object?, TResponse>(action, null, cancellationToken);
    }

    /// <summary>
    /// Sends a request with parameters
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <param name="action">The AnkiConnect action</param>
    /// <param name="parameters">The action parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response result</returns>
    public async Task<TResponse> InvokeAsync<TResponse>(
        string action,
        object? parameters,
        CancellationToken cancellationToken = default)
    {
        return await InvokeAsync<object?, TResponse>(action, parameters, cancellationToken);
    }

    /// <summary>
    /// Tests the connection to AnkiConnect
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if connection is successful</returns>
    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to get deck names as a simple connectivity test
            await InvokeAsync<IEnumerable<string>>("deckNames", cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken)
    {
        var maxRetries = _options.MaxRetries;
        var currentDelay = _options.RetryDelayMs;

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                var jsonContent = JsonContent.Create(request, options: _jsonOptions);
                var response = await _httpClient.PostAsync("", jsonContent, cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
                return result ?? throw new AnkiConnectionException("Received null response from AnkiConnect");
            }
            catch (Exception) when (attempt < maxRetries)
            {
                // Wait before retrying
                await Task.Delay(currentDelay, cancellationToken);

                // Exponential backoff
                if (_options.UseExponentialBackoff)
                {
                    currentDelay = Math.Min(currentDelay * 2, _options.MaxRetryDelayMs);
                }
            }
            catch (Exception ex)
            {
                throw new AnkiConnectionException(
                    $"Failed to communicate with AnkiConnect after {maxRetries + 1} attempts", ex);
            }
        }

        throw new AnkiConnectionException("Unexpected error in retry logic");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// AnkiConnect JSON-RPC request format
/// </summary>
internal class AnkiConnectRequest<TParams>
{
    public string Action { get; set; } = string.Empty;
    public int Version { get; set; }
    public TParams? Params { get; set; }
}

/// <summary>
/// AnkiConnect JSON-RPC response format
/// </summary>
internal class AnkiConnectResponse<TResult>
{
    public TResult? Result { get; set; }
    public AnkiConnectError? Error { get; set; }
}

/// <summary>
/// AnkiConnect error format
/// </summary>
internal class AnkiConnectError
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
}