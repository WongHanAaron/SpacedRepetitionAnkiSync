using AnkiSync.Application.Ports.Anki;
using AnkiSync.Adapter.AnkiConnect.Models;
using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly string _baseUrl = "http://127.0.0.1:8765";

    public AnkiService(HttpClient httpClient, IMapper mapper)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<TestConnectionResponse> TestConnectionAsync(TestConnectionRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<TestConnectionRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectVersionResponse>(dto.Action, dto, cancellationToken);
        return new TestConnectionResponse(response.Error == null);
    }

    /// <inheritdoc />
    public async Task<GetDecksResponse> GetDecksAsync(GetDecksRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<GetDecksRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectDeckNamesResponse>(dto.Action, dto, cancellationToken);
        return new GetDecksResponse(System.Text.Json.JsonSerializer.Deserialize<List<string>>((System.Text.Json.JsonElement)response.Result!)!);
    }

    /// <inheritdoc />
    public async Task<CreateDeckResponse> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<CreateDeckRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectCreateDeckResponse>(dto.Action, dto, cancellationToken);
        return new CreateDeckResponse(((System.Text.Json.JsonElement)response.Result!).GetInt64() > 0);
    }

    /// <inheritdoc />
    public async Task<AddNoteResponse> AddNoteAsync(AddNoteRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<AddNoteRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectAddNoteResponse>(dto.Action, dto, cancellationToken);
        return new AddNoteResponse(((System.Text.Json.JsonElement)response.Result!).GetInt64());
    }

    /// <inheritdoc />
    public async Task<UpdateNoteResponse> UpdateNoteAsync(UpdateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<UpdateNoteRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectUpdateNoteResponse>(dto.Action, dto, cancellationToken);
        return new UpdateNoteResponse(response.Result == null);
    }

    /// <inheritdoc />
    public async Task<FindNotesResponse> FindNotesAsync(FindNotesRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<FindNotesRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectFindNotesResponse>(dto.Action, dto, cancellationToken);
        return new FindNotesResponse(System.Text.Json.JsonSerializer.Deserialize<List<long>>((System.Text.Json.JsonElement)response.Result!)!);
    }

    /// <inheritdoc />
    public async Task<DeleteDecksResponse> DeleteDecksAsync(DeleteDecksRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<DeleteDecksRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectDeleteDecksResponse>(dto.Action, dto, cancellationToken);
        return new DeleteDecksResponse(response.Result != null ? (int)((System.Text.Json.JsonElement)response.Result).GetInt32() : 0);
    }

    /// <inheritdoc />
    public async Task<DeleteNotesResponse> DeleteNotesAsync(DeleteNotesRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<DeleteNotesRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectDeleteNotesResponse>(dto.Action, dto, cancellationToken);
        return new DeleteNotesResponse(response.Result != null ? (int)((System.Text.Json.JsonElement)response.Result).GetInt32() : 0);
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