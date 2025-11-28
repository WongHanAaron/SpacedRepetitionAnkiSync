using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace AnkiSync.Adapter.AnkiConnect.Client;

/// <summary>
/// Driven port for Anki operations.
/// This interface defines all interactions with the Anki application.
/// </summary>
public interface IAnkiService
{
    /// <summary>
    /// Tests the connection to Anki
    /// </summary>
    Task<VersionResponse> TestConnectionAsync(TestConnectionRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available decks from Anki
    /// </summary>
    Task<DeckNamesResponse> GetDecksAsync(GetDecksRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a deck in Anki if it doesn't exist
    /// </summary>
    Task<CreateDeckResponse> CreateDeckAsync(CreateDeckRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a note to Anki
    /// </summary>
    Task<AddNoteResponse> AddNoteAsync(AddNoteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds existing notes matching criteria
    /// </summary>
    Task<FindNotesResponse> FindNotesAsync(FindNotesRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes decks from Anki
    /// </summary>
    Task<DeleteDecksResponse> DeleteDecksAsync(DeleteDecksRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes notes from Anki
    /// </summary>
    Task<DeleteNotesResponse> DeleteNotesAsync(DeleteNotesRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a note can be added to Anki
    /// </summary>
    Task<CanAddNoteResponse> CanAddNoteAsync(CanAddNoteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a note object without saving it
    /// </summary>
    Task<CreateNoteResponse> CreateNoteAsync(CreateNoteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates fields of an existing note
    /// </summary>
    Task<UpdateNoteFieldsResponse> UpdateNoteFieldsAsync(UpdateNoteFieldsRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed information about notes
    /// </summary>
    Task<NotesInfoResponse> NotesInfoAsync(NotesInfoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed information about cards
    /// </summary>
    Task<CardsInfoResponse> CardsInfoAsync(CardsInfoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Syncs the Anki collection with AnkiWeb
    /// </summary>
    Task<SyncResponse> SyncAsync(SyncRequestDto request, CancellationToken cancellationToken = default);
}

/// <summary>
/// AnkiConnect implementation of IAnkiService
/// </summary>
public class AnkiService : IAnkiService
{
    private readonly IHttpClient _httpClient;
    private readonly string _baseUrl = "http://127.0.0.1:8765";

    public AnkiService(IHttpClient httpClient)
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
    public async Task<CardsInfoResponse> CardsInfoAsync(CardsInfoRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync<CardsInfoResponse>(request.Action, request, cancellationToken);
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
            var response = await _httpClient.ReadFromJsonAsync<TResponse>(httpResponse.Content, cancellationToken: cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException("Failed to deserialize response from AnkiConnect");
            }

            // Log the response for debugging
            var responseJson = System.Text.Json.JsonSerializer.Serialize(response);
            Console.WriteLine($"Received AnkiConnect response: {responseJson}");

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
            var rawContent = await _httpClient.ReadAsStringAsync(httpResponse.Content, cancellationToken);
            throw new InvalidOperationException($"AnkiConnect returned an unexpected response format. Raw response: {rawContent[..Math.Min(500, rawContent.Length)]}. This may indicate AnkiConnect is not properly installed or configured.", ex);
        }
    }
}