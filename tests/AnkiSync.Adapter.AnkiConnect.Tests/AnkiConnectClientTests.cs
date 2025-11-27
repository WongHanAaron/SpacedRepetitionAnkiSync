using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Configuration;
using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Domain.Core;
using AnkiSync.Domain.Core.Exceptions;
using FluentAssertions;
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
        _mockHttpClient.Setup(x => x.InvokeAsync<IEnumerable<string>>("deckNames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDecks);

        // Act
        var result = await _client.GetDecksAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedDecks);
    }

    [Fact]
    public async Task CreateDeckAsync_ValidName_CreatesDeck()
    {
        // Arrange
        var deckName = "TestDeck";
        _mockHttpClient.Setup(x => x.InvokeAsync<object>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        // Act
        await _client.CreateDeckAsync(deckName);

        // Assert
        _mockHttpClient.Verify(x => x.InvokeAsync<object>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDeckAsync_EmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        await _client.Awaiting(c => c.CreateDeckAsync(""))
            .Should().ThrowAsync<ArgumentException>();
        await _client.Awaiting(c => c.CreateDeckAsync(null!))
            .Should().ThrowAsync<ArgumentException>();
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
        result.Should().Be("Default");
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
        result.Should().Be("Math"); // Should be capitalized
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
        result.Should().BeTrue();
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
        result.Should().BeFalse();
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
        _mockHttpClient.Setup(x => x.InvokeAsync<object>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        // Mock note creation
        _mockHttpClient.Setup(x => x.InvokeAsync<long>("addNote", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(12345L);

        // Act
        var result = await _client.SyncFlashcardsAsync(flashcards);

        // Assert
        result.Success.Should().BeTrue();
        result.SyncedCount.Should().Be(2);
        result.FailedCount.Should().Be(0);
        result.ProcessedCount.Should().Be(2);
        result.Duration.TotalMilliseconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SyncFlashcardsAsync_InvalidFlashcard_RecordsError()
    {
        // Arrange
        var validFlashcard = new Flashcard { Question = "What is 2+2?", Answer = "4", SourceFile = "math.md", Tags = { "math" } };
        var invalidFlashcard = new Flashcard { Question = "", Answer = "Answer", SourceFile = "test.md" }; // Invalid - empty question

        var flashcards = new[] { validFlashcard, invalidFlashcard };

        // Mock deck creation
        _mockHttpClient.Setup(x => x.InvokeAsync<object>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        // Mock note creation for valid flashcard
        _mockHttpClient.Setup(x => x.InvokeAsync<long>("addNote", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(12345L);

        // Act
        var result = await _client.SyncFlashcardsAsync(flashcards);

        // Assert
        result.Success.Should().BeFalse();
        result.SyncedCount.Should().Be(1);
        result.FailedCount.Should().Be(1);
        result.ProcessedCount.Should().Be(2);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Flashcard question cannot be empty");
    }

    [Fact]
    public async Task SyncFlashcardsAsync_ExistingNote_UpdatesNote()
    {
        // Arrange
        var flashcard = new Flashcard { Question = "What is 2+2?", Answer = "4", SourceFile = "math.md", Tags = { "math" } };
        flashcard.AnkiNoteId = "12345"; // Mark as already synced

        var flashcards = new[] { flashcard };

        // Mock deck creation
        _mockHttpClient.Setup(x => x.InvokeAsync<object>("createDeck", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        // Mock note update
        _mockHttpClient.Setup(x => x.InvokeAsync<object>("updateNoteFields", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new object());

        // Act
        var result = await _client.SyncFlashcardsAsync(flashcards);

        // Assert
        result.Success.Should().BeTrue();
        result.SyncedCount.Should().Be(1);
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
        result.IsRunning.Should().BeFalse(); // Not applicable for direct client
        result.AnkiConnectionStatus.Should().Be(AnkiConnectionStatus.Connected);
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
        await _client.Awaiting(c => c.SyncFlashcardsAsync(flashcards, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
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