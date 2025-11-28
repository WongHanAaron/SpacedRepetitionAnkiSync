using AnkiSync.Domain;
using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using Xunit;

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
    private const string TestFront = "Test Question?";
    private const string TestBack = "Test Answer!";

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

    public Task InitializeAsync()
    {
        // No initialization needed - connection check is done in each test
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        try
        {
            // Clean up created notes (since they depend on decks)
            if (_createdNotes.Any())
            {
                try
                {
                    var deleteNotesRequest = new DeleteNotesRequestDto(_createdNotes);
                    await _ankiService.DeleteNotesAsync(deleteNotesRequest);
                }
                catch (Exception)
                {
                    // Failed to delete test notes
                }
                _createdNotes.Clear();
            }

            // Clean up created decks
            if (_createdDecks.Any())
            {
                try
                {
                    var deleteDecksRequest = new DeleteDecksRequestDto(_createdDecks, true);
                    await _ankiService.DeleteDecksAsync(deleteDecksRequest);
                }
                catch (Exception)
                {
                    // Failed to delete test decks
                }
                _createdDecks.Clear();
            }
        }
        catch (Exception)
        {
            // Log cleanup failure but don't throw - we don't want cleanup failures to mask test failures
        }
    }

    private async Task EnsureAnkiConnectionAsync()
    {
        try
        {
            var testConnectionRequest = new TestConnectionRequestDto();
            var testConnectionResponse = await _ankiService.TestConnectionAsync(testConnectionRequest);

            if (testConnectionResponse.Result == null || testConnectionResponse.Error != null)
            {
                throw new InvalidOperationException("AnkiConnect responded but returned an error or null result.");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Anki is not running or AnkiConnect is not available. Start Anki and install AnkiConnect to run these tests.", ex);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("unexpected response format"))
        {
            throw new InvalidOperationException("AnkiConnect responded but returned an unexpected format. This indicates AnkiConnect is not properly installed or configured.", ex);
        }
        catch (JsonException ex)
        {
            // This should not happen anymore since SendRequestAsync catches JsonException
            throw new InvalidOperationException("AnkiConnect responded but JSON deserialization failed. This indicates a bug in the AnkiService implementation.", ex);
        }
    }

    [Fact]
    public async Task GetDeck_WithExistingDeck_ReturnsDeckWithCards()
    {
        // Arrange - Create a test deck with a card
        var uniqueTestDeckName = $"{TestDeckName}_Download_{Guid.NewGuid():N}";
        var createDeckRequest = new CreateDeckRequestDto(uniqueTestDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest);
        _createdDecks.Add(uniqueTestDeckName);

        var testNote = new AnkiNote
        {
            DeckName = uniqueTestDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            }
        };

        // Capture time before adding note
        var timeBeforeAdd = DateTimeOffset.Now;

        var addNoteRequest = new AddNoteRequestDto(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);
        _createdNotes.Add(addNoteResponse.Result!.Value);

        // Capture time after adding note
        var timeAfterAdd = DateTimeOffset.Now;

        // Act - Download the deck
        var deckId = DeckIdExtensions.FromAnkiDeckName(uniqueTestDeckName);
        var deck = await _deckRepository.GetDeck(deckId);

        // Assert
        deck.Should().NotBeNull();
        deck.Name.Should().Be(uniqueTestDeckName);
        deck.Cards.Should().NotBeEmpty();
        deck.Cards.Should().ContainSingle();

        var card = deck.Cards.First();
        card.Should().BeOfType<QuestionAnswerCard>();
        var qaCard = (QuestionAnswerCard)card;
        qaCard.Question.Should().Be(TestFront);
        qaCard.Answer.Should().Be(TestBack);

        // Additional assertion: Verify DateModified is close to when the note was added
        var findNotesRequest = new FindNotesRequestDto($"deck:{uniqueTestDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);
        var noteIds = findNotesResponse.Result ?? new List<long>();

        var notesInfoRequest = new NotesInfoRequestDto(noteIds);
        var notesInfoResponse = await _ankiService.NotesInfoAsync(notesInfoRequest);

        notesInfoResponse.Result.Should().NotBeNull();
        notesInfoResponse.Result.Should().HaveCount(1);

        var noteInfo = notesInfoResponse.Result!.First();
        noteInfo.DateModified.Should().BeOnOrAfter(timeBeforeAdd.AddSeconds(-5)); // Allow 5 seconds grace before
        noteInfo.DateModified.Should().BeOnOrBefore(timeAfterAdd.AddSeconds(5)); // Allow 5 seconds grace after
    }

    [Fact]
    public async Task GetDeck_WithNonExistentDeck_ReturnsEmptyDeck()
    {
        // Arrange
        var nonExistentDeckName = "NonExistentDeck_12345";

        // Act
        var deckId = DeckIdExtensions.FromAnkiDeckName(nonExistentDeckName);
        var deck = await _deckRepository.GetDeck(deckId);

        // Assert
        deck.Should().NotBeNull();
        deck.Name.Should().Be(nonExistentDeckName);
        deck.Cards.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadAndDownloadDeck_ShouldPreserveContent()
    {
        // Arrange
        var uniqueDeckName = $"{TestDeckName}_RoundTrip_{Guid.NewGuid():N}";
        var originalDeck = new Deck
        {
            DeckId = DeckIdExtensions.FromAnkiDeckName(uniqueDeckName),
            Cards = new List<Card>
            {
                new QuestionAnswerCard
                {
                    Id = Guid.NewGuid().ToString(),
                    DateModified = DateTimeOffset.Now,
                    Question = "What is the capital of France?",
                    Answer = "Paris"
                },
                new QuestionAnswerCard
                {
                    Id = Guid.NewGuid().ToString(),
                    DateModified = DateTimeOffset.Now,
                    Question = "What is 2 + 2?",
                    Answer = "4"
                },
                new ClozeCard
                {
                    Id = Guid.NewGuid().ToString(),
                    DateModified = DateTimeOffset.Now,
                    Text = """The {keyword} of {country} is {city}""",
                    Answers = new Dictionary<string, string>
                    {
                        ["keyword"] = "capital",
                        ["country"] = "France",
                        ["city"] = "Paris"
                    }
                }
            }
        };

        // Act - Upload the deck
        await _deckRepository.UpsertDeck(originalDeck);

        // Track for cleanup
        _createdDecks.Add(uniqueDeckName);

        // Act - Download the deck
        var deckId = DeckIdExtensions.FromAnkiDeckName(uniqueDeckName);
        var downloadedDeck = await _deckRepository.GetDeck(deckId);

        // Assert download
        downloadedDeck.Should().NotBeNull();
        downloadedDeck.Name.Should().Be(uniqueDeckName);
        downloadedDeck.Cards.Should().HaveCount(3);

        // Verify QuestionAnswerCard 1
        var qaCard1 = downloadedDeck.Cards.OfType<QuestionAnswerCard>()
            .FirstOrDefault(c => c.Question?.Contains("France") == true);
        qaCard1.Should().NotBeNull();
        if (qaCard1 != null)
        {
            qaCard1.Question.Should().Be("What is the capital of France?");
            qaCard1.Answer.Should().Be("Paris");
        }

        // Verify QuestionAnswerCard 2
        var qaCard2 = downloadedDeck.Cards.OfType<QuestionAnswerCard>()
            .FirstOrDefault(c => c.Question?.Contains("2 + 2") == true);
        qaCard2.Should().NotBeNull();
        if (qaCard2 != null)
        {
            qaCard2.Question.Should().Be("What is 2 + 2?");
            qaCard2.Answer.Should().Be("4");
        }

        // Verify ClozeCard
        var clozeCard = downloadedDeck.Cards.OfType<ClozeCard>().FirstOrDefault();
        clozeCard.Should().NotBeNull();
        if (clozeCard != null)
        {
            clozeCard.Text.Should().Be("""The {answer1} of {answer2} is {answer3}""");
            clozeCard.Answers.Should().NotBeNull();
            clozeCard.Answers.Should().HaveCount(3);
            clozeCard.Answers.Should().ContainValues("capital", "France", "Paris");
        }
    }
}
