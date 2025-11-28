using AnkiSync.Adapter.AnkiConnect.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace AnkiSync.Adapter.AnkiConnect.Client;

/// <summary>
/// AnkiConnect implementation of IAnkiService
/// </summary>
public class AnkiService : IAnkiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://127.0.0.1:8765";

    public AnkiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<VersionResponse> TestConnectionAsync(TestConnectionRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<VersionResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<DeckNamesResponse> GetDecksAsync(GetDecksRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<DeckNamesResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<CreateDeckResponse> CreateDeckAsync(CreateDeckRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<CreateDeckResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<AddNoteResponse> AddNoteAsync(AddNoteRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<AddNoteResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<UpdateNoteResponse> UpdateNoteAsync(UpdateNoteRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<UpdateNoteResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<FindNotesResponse> FindNotesAsync(FindNotesRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<FindNotesResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<DeleteDecksResponse> DeleteDecksAsync(DeleteDecksRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<DeleteDecksResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<DeleteNotesResponse> DeleteNotesAsync(DeleteNotesRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<DeleteNotesResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<CanAddNoteResponse> CanAddNoteAsync(CanAddNoteRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<CanAddNoteResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<CreateNoteResponse> CreateNoteAsync(CreateNoteRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<CreateNoteResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<UpdateNoteFieldsResponse> UpdateNoteFieldsAsync(UpdateNoteFieldsRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<UpdateNoteFieldsResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<NotesInfoResponse> NotesInfoAsync(NotesInfoRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<NotesInfoResponse>(request.Action, request, cancellationToken);
        return response;
    }

    /// <inheritdoc />
    public async Task<SyncResponse> SyncAsync(SyncRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<SyncResponse>(request.Action, request, cancellationToken);
        return response;
    }

    private async Task<TResponse> SendRequestAsync<TResponse>(string requestUri, AnkiConnectRequest request, CancellationToken cancellationToken)
        where TResponse : class
    {
        // Log the request being sent for debugging
        var requestJson = System.Text.Json.JsonSerializer.Serialize(request);
        Console.WriteLine($"Sending AnkiConnect request: {requestJson}");

        // AnkiConnect expects all requests to be sent to the base URL, not to action-specific endpoints
        var content = new StringContent(requestJson, Encoding.UTF8);
        var httpResponse = await _httpClient.PostAsync(_baseUrl, content, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        try
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException("Failed to deserialize response from AnkiConnect");
            }

            // Check for error if the response has an Error property
            var errorProperty = typeof(TResponse).GetProperty("Error");
            if (errorProperty != null)
            {
                var error = errorProperty.GetValue(response);
                if (error != null)
                {
                    throw new InvalidOperationException($"AnkiConnect error: {error}");
                }
            }

            return response;
        }
        catch (JsonException ex)
        {
            // If JSON deserialization fails, it means AnkiConnect returned a response that's not in the expected format
            // This could be HTML error page, different JSON format, etc.
            // Let's try to read the raw response content for debugging
            var rawContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"AnkiConnect returned an unexpected response format. Raw response: {rawContent[..Math.Min(500, rawContent.Length)]}. This may indicate AnkiConnect is not properly installed or configured.", ex);
        }
    }
}