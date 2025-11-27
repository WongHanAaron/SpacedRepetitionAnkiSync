using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.AnkiConnect.Configuration;
using AnkiSync.Domain.Core;
using FluentAssertions;
using Xunit.Abstractions;

namespace AnkiSync.IntegrationTests;

/// <summary>
/// Integration tests that run against a real Anki instance
/// These tests require Anki with AnkiConnect plugin to be running
/// </summary>
[Collection("Anki Integration Tests")]
public class AnkiIntegrationTests : IClassFixture<AnkiTestFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly AnkiConnectClient _client;
    private readonly AnkiTestFixture _fixture;
    private readonly List<string> _createdDecks = new();
    private readonly List<long> _createdNotes = new();

    public AnkiIntegrationTests(ITestOutputHelper output, AnkiTestFixture fixture)
    {
        _output = output;
        _fixture = fixture;
        var options = new AnkiConnectOptions();
        _client = new AnkiConnectClient(options);
    }

    [Fact]
    public async Task AnkiConnection_ShouldBeAvailable()
    {
        _fixture.AnkiAvailable.Should().BeTrue("Anki is not available for integration testing");

        var isConnected = await _client.ValidateAnkiConnectionAsync();
        isConnected.Should().BeTrue("Should be able to connect to Anki");
    }

    [Fact]
    public async Task GetDecks_ShouldReturnDeckList()
    {
        _fixture.AnkiAvailable.Should().BeTrue("Anki is not available for integration testing");

        var decks = await _client.GetDecksAsync();
        decks.Should().NotBeNull();
        _output.WriteLine($"Found {decks.Count()} decks: {string.Join(", ", decks)}");
    }

    [Fact]
    public async Task CreateDeck_ShouldCreateNewDeck()
    {
        _fixture.AnkiAvailable.Should().BeTrue("Anki is not available for integration testing");


        var testDeckName = $"TestDeck_{DateTime.Now.Ticks}";
        _createdDecks.Add(testDeckName);

        // Create the deck
        await _client.CreateDeckAsync(testDeckName);

        // Verify it was created
        var decks = await _client.GetDecksAsync();
        decks.Should().Contain(testDeckName);

        _output.WriteLine($"Successfully created deck: {testDeckName}");
    }

    [Fact]
    public async Task SyncFlashcards_ShouldCreateNotesInAnki()
    {
        Assert.True(_fixture.AnkiAvailable, "Anki is not available for integration testing");


        // Create a test deck
        var testDeckName = $"SyncTest_{DateTime.Now.Ticks}";
        _createdDecks.Add(testDeckName);
        await _client.CreateDeckAsync(testDeckName);

        // Create test flashcards
        var flashcards = new[]
        {
            new Flashcard
            {
                Question = "What is the capital of France?",
                Answer = "Paris",
                SourceFile = "test.md",
                Tags = { "geography", "europe" }
            },
            new Flashcard
            {
                Question = "What is 2 + 2?",
                Answer = "4",
                SourceFile = "test.md",
                Tags = { "math", "basic" }
            }
        };

        // Sync the flashcards
        var result = await _client.SyncFlashcardsAsync(flashcards);

        // Verify the sync result
        result.Success.Should().BeTrue();
        result.ProcessedCount.Should().Be(2);
        result.SyncedCount.Should().Be(2);
        result.FailedCount.Should().Be(0);

        _output.WriteLine($"Sync completed successfully: {result.SyncedCount} notes synced");
    }

    [Fact]
    public async Task SyncStatus_ShouldReturnValidStatus()
    {
        _fixture.AnkiAvailable.Should().BeTrue("Anki is not available for integration testing");


        var status = await _client.GetSyncStatusAsync();

        status.Should().NotBeNull();
        status.AnkiConnectionStatus.Should().BeOneOf(AnkiConnectionStatus.Connected, AnkiConnectionStatus.Disconnected);

        _output.WriteLine($"Sync status: Running={status.IsRunning}, Connection={status.AnkiConnectionStatus}");
    }

    [Fact]
    public void DeckInference_ShouldWorkWithTags()
    {
        // Test deck inference with different tag patterns
        var flashcards = new[]
        {
            new Flashcard
            {
                Question = "Test question 1",
                Answer = "Test answer 1",
                SourceFile = "test.md",
                Tags = { "math", "algebra" }
            },
            new Flashcard
            {
                Question = "Test question 2",
                Answer = "Test answer 2",
                SourceFile = "test.md",
                Tags = { "science", "biology" }
            }
        };

        // Check deck inference
        var inferredDeck1 = _client.InferDeck(flashcards[0]);
        var inferredDeck2 = _client.InferDeck(flashcards[1]);

        inferredDeck1.Should().Be("Math");
        inferredDeck2.Should().Be("Science");

        _output.WriteLine($"Deck inference working: '{flashcards[0].Tags.First()}' -> '{inferredDeck1}'");
        _output.WriteLine($"Deck inference working: '{flashcards[1].Tags.First()}' -> '{inferredDeck2}'");
    }

    [Fact]
    public async Task ErrorHandling_ShouldHandleInvalidOperations()
    {
        _fixture.AnkiAvailable.Should().BeTrue("Anki is not available for integration testing");


        // Test creating a deck with invalid name
        await _client.Awaiting(c => c.CreateDeckAsync(""))
            .Should().ThrowAsync<ArgumentException>();

        await _client.Awaiting(c => c.CreateDeckAsync(null!))
            .Should().ThrowAsync<ArgumentException>();

        _output.WriteLine("Error handling working correctly for invalid deck names");
    }

    [Fact(Skip = "Dispose Method")]
    public void Dispose()
    {
        // Clean up created test data
        if (_fixture.AnkiAvailable)
        {
            CleanupTestDataAsync();
        }

        _client.Dispose();
    }

    private void CleanupTestDataAsync()
    {
        try
        {
            // Note: In a real implementation, you might want to clean up test decks and notes
            // But for safety, we'll leave them for manual cleanup to avoid deleting user data
            _output.WriteLine($"Test cleanup: {_createdDecks.Count} decks and {_createdNotes.Count} notes created during testing");
            _output.WriteLine("Manual cleanup may be required for test data");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Warning: Failed to cleanup test data: {ex.Message}");
        }
    }
}
