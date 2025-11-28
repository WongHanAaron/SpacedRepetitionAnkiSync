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
    public async Task<AnkiSync.Application.Ports.Anki.TestConnectionResponse> TestConnectionAsync(TestConnectionRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<TestConnectionRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.VersionResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.TestConnectionResponse(response.Error == null);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.GetDecksResponse> GetDecksAsync(GetDecksRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<GetDecksRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.DeckNamesResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.GetDecksResponse(response.Result ?? new List<string>());
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.CreateDeckResponse> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<CreateDeckRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.CreateDeckResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.CreateDeckResponse(response.Result > 0);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.AddNoteResponse> AddNoteAsync(AddNoteRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<AddNoteRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.AddNoteResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.AddNoteResponse(response.Result ?? 0);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.UpdateNoteResponse> UpdateNoteAsync(UpdateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<UpdateNoteRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.UpdateNoteResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.UpdateNoteResponse(response.Result == null);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.FindNotesResponse> FindNotesAsync(FindNotesRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<FindNotesRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.FindNotesResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.FindNotesResponse(response.Result ?? new List<long>());
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.DeleteDecksResponse> DeleteDecksAsync(DeleteDecksRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<DeleteDecksRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.DeleteDecksResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.DeleteDecksResponse(response.Result ?? 0);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.DeleteNotesResponse> DeleteNotesAsync(DeleteNotesRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<DeleteNotesRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.DeleteNotesResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.DeleteNotesResponse(response.Result ?? 0);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.CanAddNoteResponse> CanAddNoteAsync(CanAddNoteRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<CanAddNoteRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.CanAddNoteResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.CanAddNoteResponse(response.Result != null);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.CreateNoteResponse> CreateNoteAsync(CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<CreateNoteRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.CreateNoteResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.CreateNoteResponse(response.Result);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.UpdateNoteFieldsResponse> UpdateNoteFieldsAsync(UpdateNoteFieldsRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<UpdateNoteFieldsRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.UpdateNoteFieldsResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.UpdateNoteFieldsResponse(response.Error == null);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.AddTagsResponse> AddTagsAsync(AddTagsRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<AddTagsRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.AddTagsResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.AddTagsResponse(response.Error == null);
    }

    /// <inheritdoc />
    public async Task<AnkiSync.Application.Ports.Anki.GetTagsResponse> GetTagsAsync(GetTagsRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<GetTagsRequestDto>(request);
        var response = await SendRequestAsync<AnkiSync.Adapter.AnkiConnect.Models.GetTagsResponse>(dto.Action, dto, cancellationToken);
        return new AnkiSync.Application.Ports.Anki.GetTagsResponse(response.Result ?? new List<string>());
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