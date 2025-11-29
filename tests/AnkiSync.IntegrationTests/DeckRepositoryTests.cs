using AnkiSync.Domain;
using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Sdk;

namespace AnkiSync.IntegrationTests;

/// <summary>
/// Integration tests for DeckRepository against a real Anki instance.
/// These tests require Anki to be running with AnkiConnect installed.
/// </summary>
[Collection("Anki Integration Tests")]
public class DeckRepositoryTests : IAsyncLifetime
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDeckRepository _deckRepository;
    private readonly IAnkiService _ankiService;

    // Test data
    private const string TestDeckName = "AnkiSync_Test_Deck";

    // Track created resources for cleanup
    private readonly List<string> _createdDecks = new();
    private readonly List<long> _createdNotes = new();

    public DeckRepositoryTests()
    {
        var services = new ServiceCollection();
        services.AddAnkiConnectAdapter("http://127.0.0.1:8765");

        _serviceProvider = services.BuildServiceProvider();
        _deckRepository = _serviceProvider.GetRequiredService<IDeckRepository>();
        _ankiService = _serviceProvider.GetRequiredService<IAnkiService>();
    }

    private static readonly bool _isAnkiAvailable = CheckAnkiAvailability();
    
    private static bool CheckAnkiAvailability()
    {
        try
        {
            var services = new ServiceCollection();
            services.AddAnkiConnectAdapter("http://127.0.0.1:8765");
            var serviceProvider = services.BuildServiceProvider();
            var ankiService = serviceProvider.GetRequiredService<IAnkiService>();
            
            // Try to connect to Anki with a short timeout
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            ankiService.TestConnectionAsync(new TestConnectionRequestDto(), cts.Token).GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Anki is not available: {ex.Message}");
            return false;
        }
    }

    public Task InitializeAsync() => Task.CompletedTask;

    private void SkipIfAnkiNotAvailable()
    {
        if (!_isAnkiAvailable)
        {
            throw SkipException.ForSkip("Anki is not available. Make sure Anki is running with AnkiConnect installed.");
        }
    }

    public async Task DisposeAsync()
    {
        // Clean up any created notes in batches to avoid hitting request size limits
        const int batchSize = 100;
        for (int i = 0; i < _createdNotes.Count; i += batchSize)
        {
            var batch = _createdNotes.Skip(i).Take(batchSize).ToList();
            if (!batch.Any()) break;

            try
            {
                var deleteNotesRequest = new DeleteNotesRequestDto(batch);
                await _ankiService.DeleteNotesAsync(deleteNotesRequest, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clean up notes batch {i / batchSize + 1}: {ex.Message}");
            }
        }

        // Clean up any created decks
        if (_createdDecks.Any())
        {
            try
            {
                await _ankiService.DeleteDecksAsync(
                    new DeleteDecksRequestDto(_createdDecks, true), // true to delete cards too
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clean up decks: {ex.Message}");
            }
        }

        _createdDecks.Clear();
        _createdNotes.Clear();
    }

    [Fact]
    public async Task GetDeck_WithNonExistentDeck_ReturnsNull()
    {
        SkipIfAnkiNotAvailable();
            
        // Arrange - Create a unique deck name that doesn't exist
        var nonExistentDeckId = DeckIdExtensions.FromAnkiDeckName($"NonExistentDeck_{Guid.NewGuid()}");

        // Act
        var result = await _deckRepository.GetDeck(nonExistentDeckId);

        // Assert
        result.Should().BeNull("because the deck does not exist");
    }

    [Fact]
    public async Task GetDeck_WithExistingEmptyDeck_ReturnsEmptyDeck()
    {
        SkipIfAnkiNotAvailable();
            
        // Arrange - Create an empty deck
        var deckName = $"{TestDeckName}_Empty_{Guid.NewGuid()}";
        var deckId = DeckIdExtensions.FromAnkiDeckName(deckName);
        _createdDecks.Add(deckName);

        await _ankiService.CreateDeckAsync(new CreateDeckRequestDto(deckName), CancellationToken.None);

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull("because the deck exists");
        result!.DeckId.Should().Be(deckId);
        result.Cards.Should().NotBeNull().And.BeEmpty("because the deck is empty");
    }

    [Fact]
    public async Task GetDeck_WithDeckContainingCards_ReturnsDeckWithCards()
    {
        SkipIfAnkiNotAvailable();
            
        // Arrange - Create a deck with a card
        var deckName = $"{TestDeckName}_WithCards_{Guid.NewGuid()}";
        var deckId = DeckIdExtensions.FromAnkiDeckName(deckName);
        _createdDecks.Add(deckName);

        // Create the deck
        await _ankiService.CreateDeckAsync(new CreateDeckRequestDto(deckName), CancellationToken.None);
        
        // Add a note to the deck
        var addNoteRequest = new AddNoteRequestDto(new AnkiNote
        {
            DeckName = deckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = "Test Question?",
                ["Back"] = "Test Answer!"
            }
        });

        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest, CancellationToken.None);
        if (addNoteResponse.Result.HasValue)
        {
            _createdNotes.Add(addNoteResponse.Result.Value);
        }

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull("because the deck exists");
        result!.DeckId.Name.Should().Be(deckName);
        result.Cards.Should().NotBeEmpty();
        result.Cards.Should().HaveCount(1, "because we added one card");
        
        var card = result.Cards.First() as QuestionAnswerCard;
        card.Should().NotBeNull("because we added a basic note");
        card!.Question.Should().Be("Test Question?");
        card.Answer.Should().Be("Test Answer!");
    }
}
