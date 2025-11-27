using AnkiSync.Application.Ports.Anki;
using AnkiSync.Adapter.AnkiConnect.Models;
using AutoMapper;
using System.Net.Http.Json;

namespace AnkiSync.Adapter.AnkiConnect.Client;

/// <summary>
/// AnkiConnect implementation of IAnkiService
/// </summary>
public class AnkiService : IAnkiService
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

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
        return new TestConnectionResponse(response.Result > 0);
    }

    /// <inheritdoc />
    public async Task<GetDecksResponse> GetDecksAsync(GetDecksRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<GetDecksRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectDeckNamesResponse>(dto.Action, dto, cancellationToken);
        return new GetDecksResponse(response.Result);
    }

    /// <inheritdoc />
    public async Task<CreateDeckResponse> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<CreateDeckRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectCreateDeckResponse>(dto.Action, dto, cancellationToken);
        return new CreateDeckResponse(response.Result > 0);
    }

    /// <inheritdoc />
    public async Task<AddNoteResponse> AddNoteAsync(AddNoteRequest request, CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<AddNoteRequestDto>(request);
        var response = await SendRequestAsync<AnkiConnectAddNoteResponse>(dto.Action, dto, cancellationToken);
        return new AddNoteResponse(response.Result);
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
        return new FindNotesResponse(response.Result);
    }

    private async Task<TResponse> SendRequestAsync<TResponse>(string requestUri, AnkiConnectRequest request, CancellationToken cancellationToken)
        where TResponse : class
    {
        var httpResponse = await _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

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
}