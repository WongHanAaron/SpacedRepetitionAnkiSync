using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Configuration;
using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Domain.Core;
using AnkiSync.Domain.Core.Exceptions;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;

namespace AnkiSync.Adapter.AnkiConnect.Tests;

public class AnkiConnectClientTests : IDisposable
{
    private readonly Mock<IAnkiConnectHttpClient> _mockHttpClient;
    private readonly AnkiConnectOptions _options;
    private readonly AnkiConnectClient _client;
    private bool _disposed;

    public AnkiConnectClientTests()
    {
        _mockHttpClient = new Mock<IAnkiConnectHttpClient>();
        _options = new AnkiConnectOptions();
        _client = new AnkiConnectClient(_mockHttpClient.Object, _options);
    }

    [Fact]
    public async Task GetDecksAsync_ReturnsDeckNames()
    {
        // Arrange
        var expectedDecks = new List<string> { "Default", "Math", "Science" };
        _mockHttpClient.Setup(x => x.InvokeAsync<List<string>>("deckNames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDecks);

        // Act
        var result = await _client.GetDecksAsync();

        // Assert
        Assert.Equal(expectedDecks, result);
    }

    [Fact]
    public async Task CreateDeckAsync_ValidName_CreatesDeck()
    {
        // Arrange
        var deckName = "TestDeck";
        _mockHttpClient.Setup(x => x.InvokeAsync<long>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(12345L);

        // Act
        await _client.CreateDeckAsync(deckName);

        // Assert
        _mockHttpClient.Verify(x => x.InvokeAsync<long>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDeckAsync_EmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _client.CreateDeckAsync(""));
        await Assert.ThrowsAsync<ArgumentException>(() => _client.CreateDeckAsync(null!));
    }

    [Fact]
    public void InferDeck_NoTags_ReturnsDefault()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Question = "Question",
            Answer = "Answer",
            SourceFile = "test.md"
        };

        // Act
        var result = _client.InferDeck(flashcard);

        // Assert
        Assert.Equal("Default", result);
    }

    [Fact]
    public void InferDeck_WithTags_ReturnsFirstTagAsDeckName()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Question = "Question",
            Answer = "Answer",
            SourceFile = "test.md",
            Tags = { "math" }
        };

        // Act
        var result = _client.InferDeck(flashcard);

        // Assert
        Assert.Equal("Math", result); // Should be capitalized
    }

    [Fact]
    public async Task ValidateAnkiConnectionAsync_ConnectionWorks_ReturnsTrue()
    {
        // Arrange
        _mockHttpClient.Setup(x => x.TestConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _client.ValidateAnkiConnectionAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAnkiConnectionAsync_ConnectionFails_ReturnsFalse()
    {
        // Arrange
        _mockHttpClient.Setup(x => x.TestConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _client.ValidateAnkiConnectionAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SyncFlashcardsAsync_ValidFlashcards_SyncsSuccessfully()
    {
        // Arrange
        var flashcards = new[]
        {
            new Flashcard { Question = "What is 2+2?", Answer = "4", SourceFile = "math.md", Tags = { "math" } },
            new Flashcard { Question = "Capital of France?", Answer = "Paris", SourceFile = "geo.md", Tags = { "geography" } }
        };

        // Mock deck creation
        _mockHttpClient.Setup(x => x.InvokeAsync<long>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(12345L);

        // Mock note creation
        _mockHttpClient.Setup(x => x.InvokeAsync<Dictionary<string, long>>("addNote", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, long> { ["result"] = 67890L });

        // Act
        var result = await _client.SyncFlashcardsAsync(flashcards);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.SyncedCount);
        Assert.Equal(0, result.FailedCount);
        Assert.Equal(2, result.ProcessedCount);
        Assert.NotNull(result.CompletedAt);
        Assert.True(result.Duration.TotalMilliseconds > 0);
    }

    [Fact]
    public async Task SyncFlashcardsAsync_InvalidFlashcard_RecordsError()
    {
        // Arrange
        var validFlashcard = new Flashcard { Question = "What is 2+2?", Answer = "4", SourceFile = "math.md", Tags = { "math" } };
        var invalidFlashcard = new Flashcard { Question = "", Answer = "Answer", SourceFile = "test.md" }; // Invalid - empty question

        var flashcards = new[] { validFlashcard, invalidFlashcard };

        // Mock deck creation
        _mockHttpClient.Setup(x => x.InvokeAsync<long>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(12345L);

        // Mock note creation for valid flashcard
        _mockHttpClient.Setup(x => x.InvokeAsync<Dictionary<string, long>>("addNote", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, long> { ["result"] = 67890L });

        // Act
        var result = await _client.SyncFlashcardsAsync(flashcards);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(1, result.SyncedCount);
        Assert.Equal(1, result.FailedCount);
        Assert.Equal(2, result.ProcessedCount);
        Assert.Single(result.Errors);
        Assert.Contains("Flashcard question cannot be empty", result.Errors[0].Message);
    }

    [Fact]
    public async Task SyncFlashcardsAsync_ExistingNote_UpdatesNote()
    {
        // Arrange
        var flashcard = new Flashcard { Question = "What is 2+2?", Answer = "4", SourceFile = "math.md", Tags = { "math" } };
        flashcard.AnkiNoteId = "12345"; // Mark as already synced

        var flashcards = new[] { flashcard };

        // Mock deck creation
        _mockHttpClient.Setup(x => x.InvokeAsync<long>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(12345L);

        // Mock note update
        _mockHttpClient.Setup(x => x.InvokeAsync<object>("updateNoteFields", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object)null);

        // Act
        var result = await _client.SyncFlashcardsAsync(flashcards);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.SyncedCount);
        _mockHttpClient.Verify(x => x.InvokeAsync<object>("updateNoteFields", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockHttpClient.Verify(x => x.InvokeAsync<Dictionary<string, long>>("addNote", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetSyncStatusAsync_ReturnsStatus()
    {
        // Arrange
        _mockHttpClient.Setup(x => x.TestConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _client.GetSyncStatusAsync();

        // Assert
        Assert.False(result.IsRunning); // Not applicable for direct client
        Assert.Equal(AnkiConnectionStatus.Connected, result.AnkiConnectionStatus);
    }

    [Fact]
    public async Task SyncFlashcardsAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var flashcards = new[] { new Flashcard { Question = "Question", Answer = "Answer", SourceFile = "test.md", Tags = { "test" } } };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Mock to throw OperationCanceledException when cancellation is requested
        _mockHttpClient.Setup(x => x.InvokeAsync<long>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _client.SyncFlashcardsAsync(flashcards, cts.Token));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _client.Dispose();
            _disposed = true;
        }
    }
}