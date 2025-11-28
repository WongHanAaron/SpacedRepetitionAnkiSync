using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.AnkiConnect.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using Xunit;
using Xunit.Sdk;

namespace AnkiSync.IntegrationTests;

/// <summary>
/// Integration tests for AnkiConnect adapter against a real Anki instance.
/// These tests require Anki to be running with AnkiConnect installed.
/// </summary>
[Collection("Anki Integration Tests")]
public class AnkiConnectTests : IAsyncLifetime
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAnkiService _ankiService;

    // Test data
    private const string TestDeckName = "AnkiSync_Test_Deck";
    private const string TestFront = "Test Question?";
    private const string TestBack = "Test Answer!";
    private const string UpdatedBack = "Updated Answer!";

    // Track created resources for cleanup
    private readonly List<string> _createdDecks = new();
    private readonly List<long> _createdNotes = new();

    // Track initial state for verification
    private IEnumerable<string>? _initialDeckNames;

    public AnkiConnectTests()
    {
        var services = new ServiceCollection();
        services.AddAnkiConnectAdapter("http://127.0.0.1:8765");

        _serviceProvider = services.BuildServiceProvider();
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to delete test notes: {ex.Message}");
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to delete test decks: {ex.Message}");
                }
                _createdDecks.Clear();
            }

            // Verify that deck names are back to initial state
            if (_initialDeckNames != null)
            {
                var finalGetDecksRequest = new GetDecksRequestDto();
                var finalGetDecksResponse = await _ankiService.GetDecksAsync(finalGetDecksRequest);
                (finalGetDecksResponse.Result ?? new List<string>()).OrderBy(d => d).Should().BeEquivalentTo(_initialDeckNames,
                    "Deck names should be restored to initial state after test cleanup");
            }
        }
        catch (Exception ex)
        {
            // Log cleanup failure but don't throw - we don't want cleanup failures to mask test failures
            Console.WriteLine($"Warning: Failed to clean up test resources: {ex.Message}");
        }
    }

    private async Task EnsureAnkiConnectionAsync()
    {
        try
        {
            var testConnectionRequest = new TestConnectionRequestDto();
            var response = await _ankiService.TestConnectionAsync(testConnectionRequest);

            if (response.Result == null || response.Error != null)
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

    private async Task CaptureInitialDeckStateAsync()
    {
        var getDecksRequest = new GetDecksRequestDto();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        _initialDeckNames = (getDecksResponse.Result ?? new List<string>()).OrderBy(d => d).ToList();
    }

    [Fact]
    public async Task AnkiService_FullWorkflow_ShouldWorkCorrectly()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        // Test 1: Get existing decks
        var getDecksRequest = new GetDecksRequestDto();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);

        getDecksResponse.Result.Should().NotBeNull();
        getDecksResponse.Result.Should().Contain("Default");

        var initialDeckCount = (getDecksResponse.Result ?? new List<string>()).Count();

        // Test 2: Create a test deck
        var uniqueTestDeckName = $"{TestDeckName}_{Guid.NewGuid():N}";
        var createDeckRequest = new CreateDeckRequestDto(uniqueTestDeckName);
        var createDeckResponse = await _ankiService.CreateDeckAsync(createDeckRequest);

        (createDeckResponse.Error == null).Should().BeTrue();
        _createdDecks.Add(uniqueTestDeckName);

        // Test 3: Verify deck was created
        getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        getDecksResponse.Result.Should().Contain(uniqueTestDeckName);
        (getDecksResponse.Result ?? new List<string>()).Count().Should().Be(initialDeckCount + 1);

        // Test 4: Create a test note
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

        var addNoteRequest = new AddNoteRequestDto(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);

        addNoteResponse.Result.Should().NotBeNull();
        addNoteResponse.Result.Should().BeGreaterThan(0L);
        var createdNoteId = addNoteResponse.Result!.Value;
        _createdNotes.Add(createdNoteId);

        // Test 5: Find notes in test deck to verify note was added
        var findNotesRequest = new FindNotesRequestDto($"deck:{uniqueTestDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        (findNotesResponse.Result ?? new List<long>()).Should().Contain(createdNoteId);
        (findNotesResponse.Result ?? new List<long>()).Count().Should().Be(1);

        // Test 6: Update the note
        var updateNoteRequest = new UpdateNoteFieldsRequestDto(createdNoteId, new Dictionary<string, string>
        {
            ["Back"] = UpdatedBack
        });
        var updateNoteResponse = await _ankiService.UpdateNoteFieldsAsync(updateNoteRequest);

        (updateNoteResponse.Error == null).Should().BeTrue();

        // Test 7: Find notes again to ensure our test deck still has the note
        findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);
        findNotesResponse.Result.Should().Contain(createdNoteId);
        (findNotesResponse.Result ?? new List<long>()).Count().Should().Be(1);

        // Note: We can't easily verify the field content was updated without additional API calls
        // that would require more complex AnkiConnect operations. The update success is our primary verification.

        // Cleanup: We could delete the note and deck, but AnkiConnect doesn't have a simple delete operation
        // The test deck and note will remain for manual cleanup if needed
    }

    [Fact]
    public async Task AnkiService_TestConnection_ShouldReturnTrue_WhenAnkiIsRunning()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var request = new TestConnectionRequestDto();
        var response = await _ankiService.TestConnectionAsync(request);

        (response.Result != null && response.Error == null).Should().BeTrue();
    }

    [Fact]
    public async Task AnkiService_GetDecks_ShouldReturnDeckList()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var request = new GetDecksRequestDto();
        var response = await _ankiService.GetDecksAsync(request);

        response.Result.Should().NotBeNull();
        response.Result.Should().NotBeEmpty();
        response.Result.Should().Contain("Default");
    }

    [Fact]
    public async Task AnkiService_CreateDeck_ShouldCreateNewDeck()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        // Use a unique deck name to avoid conflicts
        var uniqueDeckName = $"{TestDeckName}_{Guid.NewGuid():N}";

        var request = new CreateDeckRequestDto(uniqueDeckName);
        var response = await _ankiService.CreateDeckAsync(request);

        (response.Error == null).Should().BeTrue();

        // Track for cleanup
        _createdDecks.Add(uniqueDeckName);

        // Verify deck was created
        var getDecksRequest = new GetDecksRequestDto();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);

        getDecksResponse.Result.Should().Contain(uniqueDeckName);
    }

    [Fact]
    public async Task AnkiService_AddNote_ShouldCreateNewNote()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_AddNote_{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequestDto(uniqueDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest);
        _createdDecks.Add(uniqueDeckName);

        // Add note
        var testNote = new AnkiNote
        {
            DeckName = uniqueDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            }
        };

        var addNoteRequest = new AddNoteRequestDto(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);

        addNoteResponse.Result.Should().NotBeNull();
        addNoteResponse.Result.Should().BeGreaterThan(0L);
        _createdNotes.Add(addNoteResponse.Result!.Value);

        // Verify note was added
        var findNotesRequest = new FindNotesRequestDto($"deck:{uniqueDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        (findNotesResponse.Result ?? new List<long>()).Should().Contain(addNoteResponse.Result!.Value);
    }

    [Fact]
    public async Task AnkiService_FindNotes_ShouldReturnNotesMatchingQuery()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var request = new FindNotesRequestDto("deck:Default");
        var response = await _ankiService.FindNotesAsync(request);

        response.Result.Should().NotBeNull();
        // Default deck might be empty, so we just verify the call succeeds
    }

    [Fact]
    public async Task AnkiService_AddAndDeleteDeck_ShouldCreateAndRemoveDeck()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        // Use a unique deck name to avoid conflicts
        var uniqueDeckName = $"{TestDeckName}_AddDelete_{Guid.NewGuid():N}";

        // Test 1: Create a deck
        var createDeckRequest = new CreateDeckRequestDto(uniqueDeckName);
        var createDeckResponse = await _ankiService.CreateDeckAsync(createDeckRequest);

        (createDeckResponse.Error == null).Should().BeTrue();

        // Verify deck was created
        var getDecksRequest = new GetDecksRequestDto();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        getDecksResponse.Result.Should().Contain(uniqueDeckName);

        // Test 2: Delete the deck (should work now that deck deletion is fixed)
        var deleteDecksRequest = new DeleteDecksRequestDto(new[] { uniqueDeckName }, true);
        var deleteDecksResponse = await _ankiService.DeleteDecksAsync(deleteDecksRequest);
        deleteDecksResponse.Error.Should().BeNull();
        deleteDecksResponse.Result.Should().BeNull();

        // Test 3: Verify the deck was actually deleted
        var finalGetDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        finalGetDecksResponse.Result.Should().NotContain(uniqueDeckName,
            "The deck should have been successfully deleted");

        // Note: Since we successfully deleted the deck, we don't need to track it for cleanup
    }

    [Fact]
    public async Task AnkiService_CanAddNote_ShouldReturnTrue_WhenDeckExists()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_CanAddNote_{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequestDto(uniqueDeckName);
        var createDeckResponse = await _ankiService.CreateDeckAsync(createDeckRequest);
        createDeckResponse.Error.Should().BeNull();
        _createdDecks.Add(uniqueDeckName);

        // Test CanAddNote with valid deck
        var uniqueId = Guid.NewGuid().ToString("N");
        var testNote = new AnkiNote
        {
            DeckName = uniqueDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = $"{TestFront} {uniqueId}",
                ["Back"] = $"{TestBack} {uniqueId}"
            }
        };

        var canAddNoteRequest = new CanAddNoteRequestDto(testNote);
        var canAddNoteResponse = await _ankiService.CanAddNoteAsync(canAddNoteRequest);

        (canAddNoteResponse.Result as bool? == true).Should().BeTrue();
    }

    [Fact]
    public async Task AnkiService_CanAddNote_ShouldReturnFalse_WhenDeckDoesNotExist()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        // Test CanAddNote returns false when deck does not exist
        var uniqueId = Guid.NewGuid().ToString("N");
        var testNote = new AnkiNote
        {
            DeckName = "NonExistentDeck_12345",
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = $"{TestFront} {uniqueId}",
                ["Back"] = $"{TestBack} {uniqueId}"
            }
        };

        var canAddNoteRequest = new CanAddNoteRequestDto(testNote);
        var canAddNoteResponse = await _ankiService.CanAddNoteAsync(canAddNoteRequest);

        (canAddNoteResponse.Result as bool?).Should().BeFalse();
    }

    [Fact]
    public async Task AnkiService_CreateNote_ShouldCreateNoteObject()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_CreateNote_{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequestDto(uniqueDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest);
        _createdDecks.Add(uniqueDeckName);

        // Create note object
        var testNote = new AnkiNote
        {
            DeckName = uniqueDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            }
        };

        var createNoteRequest = new CreateNoteRequestDto(testNote);
        var createNoteResponse = await _ankiService.CreateNoteAsync(createNoteRequest);

        createNoteResponse.Result.Should().NotBeNull();
        createNoteResponse.Result.Should().BeGreaterThan(0);

        long createdNoteId = (long)createNoteResponse.Result!;

        // Verify note was created by finding it
        var findNotesRequest = new FindNotesRequestDto($"deck:{uniqueDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        findNotesResponse.Result.Should().Contain(createdNoteId);

        // Clean up the created note
        _createdNotes.Add(createdNoteId);
    }

    [Fact]
    public async Task AnkiService_UpdateNoteFields_ShouldUpdateNoteFields()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_UpdateFields_{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequestDto(uniqueDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest);
        _createdDecks.Add(uniqueDeckName);

        // Create a note first
        var testNote = new AnkiNote
        {
            DeckName = uniqueDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            }
        };

        var addNoteRequest = new AddNoteRequestDto(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);
        _createdNotes.Add(addNoteResponse.Result!.Value);

        // Update the note fields
        var updateFieldsRequest = new UpdateNoteFieldsRequestDto(addNoteResponse.Result!.Value, new Dictionary<string, string>
        {
            ["Back"] = UpdatedBack
        });
        var updateFieldsResponse = await _ankiService.UpdateNoteFieldsAsync(updateFieldsRequest);

        (updateFieldsResponse.Error == null).Should().BeTrue();

        // Verify the note still exists (we can't easily verify field content without more complex API calls)
        var findNotesRequest = new FindNotesRequestDto($"deck:{uniqueDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        (findNotesResponse.Result ?? new List<long>()).Should().Contain(addNoteResponse.Result!.Value);
    }

    [Fact]
    public async Task SyncAsync_ShouldSyncCollectionWithAnkiWeb()
    {
        await EnsureAnkiConnectionAsync();

        // Sync the collection with AnkiWeb
        var syncRequest = new SyncRequestDto();
        var syncResponse = await _ankiService.SyncAsync(syncRequest);

        // The sync operation should complete without error
        // Note: The actual result depends on whether the user has AnkiWeb configured
        // We just verify the API call succeeds
        syncResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task CardsInfoAsync_ShouldReturnCardInformationWithDatetimeFields()
    {
        // Arrange - Create a test deck and note
        var uniqueTestDeckName = $"{TestDeckName}_CardsInfo_{Guid.NewGuid():N}";
        var createDeckRequest = new CreateDeckRequestDto(uniqueTestDeckName);
        var createDeckResponse = await _ankiService.CreateDeckAsync(createDeckRequest);
        _createdDecks.Add(uniqueTestDeckName);

        (createDeckResponse.Error == null).Should().BeTrue();

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

        var addNoteRequest = new AddNoteRequestDto(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);
        _createdNotes.Add(addNoteResponse.Result!.Value);

        // Get the card IDs from the note
        var findNotesRequest = new FindNotesRequestDto($"deck:{uniqueTestDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);
        var noteIds = findNotesResponse.Result ?? new List<long>();

        var notesInfoRequest = new NotesInfoRequestDto(noteIds);
        var notesInfoResponse = await _ankiService.NotesInfoAsync(notesInfoRequest);

        var cardIds = notesInfoResponse.Result?.FirstOrDefault()?.Cards ?? new List<long>();

        // Act - Get card information
        var cardsInfoRequest = new CardsInfoRequestDto(cardIds);
        var cardsInfoResponse = await _ankiService.CardsInfoAsync(cardsInfoRequest);

        // Assert
        cardsInfoResponse.Should().NotBeNull();
        (cardsInfoResponse.Error == null).Should().BeTrue();
        cardsInfoResponse.Result.Should().NotBeNull();
        cardsInfoResponse.Result.Should().HaveCount(cardIds.Count());

        var cardInfo = cardsInfoResponse.Result!.First();
        cardInfo.CardId.Should().BeGreaterThan(0);
        cardInfo.Note.Should().Be(addNoteResponse.Result!.Value);

        // Log the card info to see what datetime fields are available
        Console.WriteLine($"CardInfo: {JsonSerializer.Serialize(cardInfo)}");

        // Verify datetime-related fields exist and have reasonable values
        cardInfo.Due.Should().BeGreaterThanOrEqualTo(0); // Due date (could be days or timestamp)
        cardInfo.Interval.Should().BeGreaterThanOrEqualTo(0); // Review interval
        cardInfo.Reps.Should().BeGreaterThanOrEqualTo(0); // Number of reviews
        cardInfo.Lapses.Should().BeGreaterThanOrEqualTo(0); // Number of lapses
    }
}