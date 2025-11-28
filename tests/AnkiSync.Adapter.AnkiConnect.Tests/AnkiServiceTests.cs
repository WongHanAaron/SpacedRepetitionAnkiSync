using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AnkiSync.Adapter.AnkiConnect.Tests;

/// <summary>
/// Unit tests for AnkiService using mocked IHttpClient
/// </summary>
public class AnkiServiceTests
{
    private readonly Mock<IHttpClient> _httpClientMock;
    private readonly AnkiService _ankiService;

    public AnkiServiceTests()
    {
        _httpClientMock = new Mock<IHttpClient>();
        _ankiService = new AnkiService(_httpClientMock.Object);
    }

    [Fact]
    public async Task TestConnectionAsync_ShouldReturnVersionResponse()
    {
        // Arrange
        var request = new TestConnectionRequestDto();
        var expectedResponse = new VersionResponse { Result = 6 };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.TestConnectionAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(6);
        VerifyRequestSent("version");
    }

    [Fact]
    public async Task GetDecksAsync_ShouldReturnDeckNamesResponse()
    {
        // Arrange
        var request = new GetDecksRequestDto();
        var expectedResponse = new DeckNamesResponse { Result = new List<string> { "Default", "TestDeck" } };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.GetDecksAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull();
        result.Result.Should().Contain("Default");
        result.Result.Should().Contain("TestDeck");
        VerifyRequestSent("deckNames");
    }

    [Fact]
    public async Task CreateDeckAsync_ShouldReturnCreateDeckResponse()
    {
        // Arrange
        var request = new CreateDeckRequestDto("TestDeck");
        var expectedResponse = new CreateDeckResponse { Result = 123456789L };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.CreateDeckAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(123456789L);
        VerifyRequestSent("createDeck", "{\"deck\":\"TestDeck\"}");
    }

    [Fact]
    public async Task AddNoteAsync_ShouldReturnAddNoteResponse()
    {
        // Arrange
        var note = new AnkiNote
        {
            DeckName = "TestDeck",
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = "Question?",
                ["Back"] = "Answer!"
            }
        };
        var request = new AddNoteRequestDto(note);
        var expectedResponse = new AddNoteResponse { Result = 987654321L };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.AddNoteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(987654321L);
        VerifyRequestSent("addNote", $"{{\"note\":{{\"deckName\":\"TestDeck\",\"modelName\":\"Basic\",\"fields\":{{\"Front\":\"Question?\",\"Back\":\"Answer!\"}}}}}}");
    }

    [Fact]
    public async Task FindNotesAsync_ShouldReturnFindNotesResponse()
    {
        // Arrange
        var request = new FindNotesRequestDto("deck:TestDeck");
        var expectedResponse = new FindNotesResponse { Result = new List<long> { 123, 456, 789 } };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.FindNotesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull();
        result.Result.Should().HaveCount(3);
        result.Result.Should().Contain(123);
        result.Result.Should().Contain(456);
        result.Result.Should().Contain(789);
        VerifyRequestSent("findNotes", "{\"query\":\"deck:TestDeck\"}");
    }

    [Fact]
    public async Task DeleteDecksAsync_ShouldReturnDeleteDecksResponse()
    {
        // Arrange
        var request = new DeleteDecksRequestDto(new[] { "TestDeck" }, true);
        var expectedResponse = new DeleteDecksResponse { Result = null };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.DeleteDecksAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeNull();
        VerifyRequestSent("deleteDecks", "{\"decks\":[\"TestDeck\"],\"cardsToo\":true}");
    }

    [Fact]
    public async Task DeleteNotesAsync_ShouldReturnDeleteNotesResponse()
    {
        // Arrange
        var request = new DeleteNotesRequestDto(new[] { 123L, 456L });
        var expectedResponse = new DeleteNotesResponse { Result = null };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.DeleteNotesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeNull();
        VerifyRequestSent("deleteNotes", "{\"notes\":[123,456]}");
    }

    [Fact]
    public async Task CanAddNoteAsync_ShouldReturnTrue_WhenDeckExists()
    {
        // Arrange
        var note = new AnkiNote
        {
            DeckName = "TestDeck",
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = "Question?",
                ["Back"] = "Answer!"
            }
        };
        var request = new CanAddNoteRequestDto(note);
        var expectedResponse = new CanAddNoteResponse { Result = true };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.CanAddNoteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(true);
        VerifyRequestSent("canAddNote", $"{{\"note\":{{\"deckName\":\"TestDeck\",\"modelName\":\"Basic\",\"fields\":{{\"Front\":\"Question?\",\"Back\":\"Answer!\"}}}}}}");
    }

    [Fact]
    public async Task CreateNoteAsync_ShouldReturnCreateNoteResponse()
    {
        // Arrange
        var note = new AnkiNote
        {
            DeckName = "TestDeck",
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = "Question?",
                ["Back"] = "Answer!"
            }
        };
        var request = new CreateNoteRequestDto(note);
        var expectedResponse = new CreateNoteResponse { Result = 987654321L };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.CreateNoteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(987654321L);
        VerifyRequestSent("addNote", $"{{\"note\":{{\"deckName\":\"TestDeck\",\"modelName\":\"Basic\",\"fields\":{{\"Front\":\"Question?\",\"Back\":\"Answer!\"}}}}}}");
    }

    [Fact]
    public async Task UpdateNoteFieldsAsync_ShouldReturnUpdateNoteFieldsResponse()
    {
        // Arrange
        var request = new UpdateNoteFieldsRequestDto(123, new Dictionary<string, string> { ["Back"] = "Updated!" });
        var expectedResponse = new UpdateNoteFieldsResponse { Result = null };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.UpdateNoteFieldsAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeNull();
        VerifyRequestSent("updateNoteFields", "{\"note\":{\"id\":123,\"fields\":{\"Back\":\"Updated!\"}}}");
    }

    [Fact]
    public async Task NotesInfoAsync_ShouldReturnNotesInfoResponse()
    {
        // Arrange
        var request = new NotesInfoRequestDto(new[] { 123L, 456L });
        var expectedResponse = new NotesInfoResponse
        {
            Result = new List<NoteInfo>
            {
                new NoteInfo { NoteId = 123, ModelName = "Basic", Fields = new Dictionary<string, NoteFieldInfo> { ["Front"] = new NoteFieldInfo { Value = "Q1" }, ["Back"] = new NoteFieldInfo { Value = "A1" } } },
                new NoteInfo { NoteId = 456, ModelName = "Basic", Fields = new Dictionary<string, NoteFieldInfo> { ["Front"] = new NoteFieldInfo { Value = "Q2" }, ["Back"] = new NoteFieldInfo { Value = "A2" } } }
            }
        };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.NotesInfoAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull();
        result.Result.Should().HaveCount(2);
        result.Result![0].NoteId.Should().Be(123);
        result.Result![1].NoteId.Should().Be(456);
        VerifyRequestSent("notesInfo", "{\"notes\":[123,456]}");
    }

    [Fact]
    public async Task NotesInfoAsync_ShouldDeserializeModFieldToDateModified()
    {
        // Arrange
        var request = new NotesInfoRequestDto(new[] { 123L });
        var expectedResponse = new NotesInfoResponse
        {
            Result = new List<NoteInfo>
            {
                new NoteInfo 
                { 
                    NoteId = 123, 
                    ModelName = "Basic", 
                    ModificationTimestamp = 1764349536,
                    Fields = new Dictionary<string, NoteFieldInfo> { ["Front"] = new NoteFieldInfo { Value = "Q1" }, ["Back"] = new NoteFieldInfo { Value = "A1" } } 
                }
            }
        };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.NotesInfoAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull();
        result.Result.Should().HaveCount(1);
        result.Result![0].ModificationTimestamp.Should().Be(1764349536);
        result.Result![0].DateModified.Should().Be(DateTimeOffset.FromUnixTimeSeconds(1764349536));
        VerifyRequestSent("notesInfo", "{\"notes\":[123]}");
    }

    [Fact]
    public async Task SyncAsync_ShouldReturnSyncResponse()
    {
        // Arrange
        var request = new SyncRequestDto();
        var expectedResponse = new SyncResponse { Result = null };
        SetupHttpClientMock(expectedResponse);

        // Act
        var result = await _ankiService.SyncAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeNull();
        VerifyRequestSent("sync");
    }

    [Fact]
    public async Task SendRequestAsync_ShouldThrowException_WhenHttpRequestFails()
    {
        // Arrange
        var request = new TestConnectionRequestDto();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _httpClientMock
            .Setup(x => x.PostAsync("http://127.0.0.1:8765", It.IsAny<StringContent>(), default))
            .ReturnsAsync(httpResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _ankiService.TestConnectionAsync(request));
    }

    [Fact]
    public async Task SendRequestAsync_ShouldThrowException_WhenAnkiConnectReturnsError()
    {
        // Arrange
        var request = new TestConnectionRequestDto();
        var expectedResponse = new VersionResponse { Error = "Some AnkiConnect error" };
        SetupHttpClientMock(expectedResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _ankiService.TestConnectionAsync(request));
        exception.Message.Should().Contain("AnkiConnect error: Some AnkiConnect error");
    }

    [Fact]
    public async Task SendRequestAsync_ShouldThrowException_WhenJsonDeserializationFails()
    {
        // Arrange
        var request = new TestConnectionRequestDto();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponse.Content = new StringContent("Invalid JSON response");

        _httpClientMock
            .Setup(x => x.PostAsync("http://127.0.0.1:8765", It.IsAny<HttpContent>(), default))
            .ReturnsAsync(httpResponse);

        _httpClientMock
            .Setup(x => x.ReadFromJsonAsync<VersionResponse>(httpResponse.Content, null, default))
            .ThrowsAsync(new JsonException("Invalid JSON"));

        _httpClientMock
            .Setup(x => x.ReadAsStringAsync(httpResponse.Content, default))
            .ReturnsAsync("Invalid JSON response");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _ankiService.TestConnectionAsync(request));
        exception.Message.Should().Contain("AnkiConnect returned an unexpected response format");
    }

    private static HttpResponseMessage CreateHttpResponse<T>(T response)
    {
        var json = JsonSerializer.Serialize(response);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return httpResponse;
    }

    private StringContent? _capturedContent;

    private void SetupHttpClientMock<TResponse>(TResponse response) where TResponse : class
    {
        var httpResponse = CreateHttpResponse(response);
        _httpClientMock
            .Setup(x => x.PostAsync("http://127.0.0.1:8765", It.IsAny<HttpContent>(), default))
            .Callback<string, HttpContent, CancellationToken>((url, content, token) => _capturedContent = content as StringContent)
            .ReturnsAsync(httpResponse);

        _httpClientMock
            .Setup(x => x.ReadFromJsonAsync<TResponse>(httpResponse.Content, null, default))
            .ReturnsAsync(response);
    }

    private void VerifyRequestSent(string expectedAction, string? expectedParams = null)
    {
        _capturedContent.Should().NotBeNull();
        VerifyRequestContent(_capturedContent!, expectedAction, expectedParams).Should().BeTrue();
    }

    private static bool VerifyRequestContent(StringContent content, string expectedAction, string? expectedParams)
    {
        var jsonString = content.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(jsonString);
        var root = doc.RootElement;

        // Verify action
        if (!root.TryGetProperty("action", out var actionElement) || actionElement.GetString() != expectedAction)
            return false;

        // Verify version
        if (!root.TryGetProperty("version", out var versionElement) || versionElement.GetInt32() != 6)
            return false;

        // If params are expected, verify them
        if (expectedParams != null)
        {
            if (!root.TryGetProperty("params", out var paramsElement))
                return false;

            var paramsJson = paramsElement.GetRawText();
            return paramsJson == expectedParams;
        }

        return true;
    }
}