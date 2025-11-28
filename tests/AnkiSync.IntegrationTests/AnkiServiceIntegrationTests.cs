using AnkiSync.Application.Ports.Anki;
using AnkiSync.Domain;
using AnkiSync.Adapter.AnkiConnect;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using Xunit;
using Xunit.Sdk;

namespace AnkiSync.IntegrationTests;

/// <summary>
/// Integration tests for AnkiService against a real Anki instance.
/// These tests require Anki to be running with AnkiConnect installed.
/// </summary>
public class AnkiServiceIntegrationTests : IAsyncLifetime
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

    public AnkiServiceIntegrationTests()
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

    private async Task EnsureAnkiConnectionAsync()
    {
        try
        {
            var connectionRequest = new TestConnectionRequest();
            var connectionResponse = await _ankiService.TestConnectionAsync(connectionRequest);

            if (!connectionResponse.IsConnected)
            {
                throw new InvalidOperationException("Anki is not running or AnkiConnect is not available. Start Anki and install AnkiConnect to run these tests.");
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

    public async Task DisposeAsync()
    {
        try
        {
            // Clean up created notes first (since they depend on decks)
            if (_createdNotes.Any())
            {
                try
                {
                    var deleteNotesRequest = new DeleteNotesRequest(_createdNotes);
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
                    var deleteDecksRequest = new DeleteDecksRequest(_createdDecks, true);
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
                var finalGetDecksRequest = new GetDecksRequest();
                var finalGetDecksResponse = await _ankiService.GetDecksAsync(finalGetDecksRequest);
                finalGetDecksResponse.DeckNames.OrderBy(d => d).Should().BeEquivalentTo(_initialDeckNames, 
                    "Deck names should be restored to initial state after test cleanup");
            }

            // Verify that no test notes remain (check for notes with integration-test tag)
            var testNotesRequest = new FindNotesRequest("tag:integration-test");
            var testNotesResponse = await _ankiService.FindNotesAsync(testNotesRequest);
            testNotesResponse.NoteIds.Should().BeEmpty("No test notes should remain after cleanup");
        }
        catch (Exception ex)
        {
            // Log cleanup failure but don't throw - we don't want cleanup failures to mask test failures
            Console.WriteLine($"Warning: Failed to clean up test resources: {ex.Message}");
        }
    }

    private async Task CaptureInitialDeckStateAsync()
    {
        var getDecksRequest = new GetDecksRequest();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        _initialDeckNames = getDecksResponse.DeckNames.OrderBy(d => d).ToList();
    }

    [Fact]
    public async Task AnkiService_FullWorkflow_ShouldWorkCorrectly()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        // Test 1: Get existing decks
        var getDecksRequest = new GetDecksRequest();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);

        getDecksResponse.DeckNames.Should().NotBeNull();
        getDecksResponse.DeckNames.Should().Contain("Default");

        var initialDeckCount = getDecksResponse.DeckNames.Count();

        // Test 2: Create a test deck
        var uniqueTestDeckName = $"{TestDeckName}_{Guid.NewGuid():N}";
        var createDeckRequest = new CreateDeckRequest(uniqueTestDeckName);
        var createDeckResponse = await _ankiService.CreateDeckAsync(createDeckRequest);

        createDeckResponse.Success.Should().BeTrue();
        _createdDecks.Add(uniqueTestDeckName);

        // Test 3: Verify deck was created
        getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        getDecksResponse.DeckNames.Should().Contain(uniqueTestDeckName);
        getDecksResponse.DeckNames.Count().Should().Be(initialDeckCount + 1);

        // Test 4: Create a test note
        var testNote = new AnkiNote
        {
            DeckName = uniqueTestDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            },
            Tags = new List<string> { "integration-test" }
        };

        var addNoteRequest = new AddNoteRequest(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);

        addNoteResponse.NoteId.Should().BeGreaterThan(0);
        var createdNoteId = addNoteResponse.NoteId;
        _createdNotes.Add(createdNoteId);

        // Test 5: Find notes in test deck to verify note was added
        var findNotesRequest = new FindNotesRequest($"deck:{uniqueTestDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        findNotesResponse.NoteIds.Should().Contain(createdNoteId);
        findNotesResponse.NoteIds.Count().Should().Be(1);

        // Test 6: Update the note
        var updateNoteRequest = new UpdateNoteRequest(createdNoteId, new Dictionary<string, string>
        {
            ["Back"] = UpdatedBack
        });
        var updateNoteResponse = await _ankiService.UpdateNoteAsync(updateNoteRequest);

        updateNoteResponse.Success.Should().BeTrue();

        // Test 7: Find notes again to ensure our test deck still has the note
        findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);
        findNotesResponse.NoteIds.Should().Contain(createdNoteId);
        findNotesResponse.NoteIds.Count().Should().Be(1);

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

        var request = new TestConnectionRequest();
        var response = await _ankiService.TestConnectionAsync(request);

        response.IsConnected.Should().BeTrue();
    }

    [Fact]
    public async Task AnkiService_GetDecks_ShouldReturnDeckList()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var request = new GetDecksRequest();
        var response = await _ankiService.GetDecksAsync(request);

        response.DeckNames.Should().NotBeNull();
        response.DeckNames.Should().NotBeEmpty();
        response.DeckNames.Should().Contain("Default");
    }

    [Fact]
    public async Task AnkiService_CreateDeck_ShouldCreateNewDeck()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        // Use a unique deck name to avoid conflicts
        var uniqueDeckName = $"{TestDeckName}_{Guid.NewGuid():N}";

        var request = new CreateDeckRequest(uniqueDeckName);
        var response = await _ankiService.CreateDeckAsync(request);

        response.Success.Should().BeTrue();

        // Track for cleanup
        _createdDecks.Add(uniqueDeckName);

        // Verify deck was created
        var getDecksRequest = new GetDecksRequest();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);

        getDecksResponse.DeckNames.Should().Contain(uniqueDeckName);
    }

    [Fact]
    public async Task AnkiService_AddNote_ShouldCreateNewNote()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_AddNote_{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequest(uniqueDeckName);
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
            },
            Tags = new List<string> { "integration-test" }
        };

        var addNoteRequest = new AddNoteRequest(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);

        addNoteResponse.NoteId.Should().BeGreaterThan(0);
        _createdNotes.Add(addNoteResponse.NoteId);

        // Verify note was added
        var findNotesRequest = new FindNotesRequest($"deck:{uniqueDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        findNotesResponse.NoteIds.Should().Contain(addNoteResponse.NoteId);
    }

    [Fact]
    public async Task AnkiService_FindNotes_ShouldReturnNotesMatchingQuery()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var request = new FindNotesRequest("deck:Default");
        var response = await _ankiService.FindNotesAsync(request);

        response.NoteIds.Should().NotBeNull();
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
        var createDeckRequest = new CreateDeckRequest(uniqueDeckName);
        var createDeckResponse = await _ankiService.CreateDeckAsync(createDeckRequest);

        createDeckResponse.Success.Should().BeTrue();

        // Verify deck was created
        var getDecksRequest = new GetDecksRequest();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        getDecksResponse.DeckNames.Should().Contain(uniqueDeckName);

        // Test 2: Delete the deck (should work now that deck deletion is fixed)
        var deleteDecksRequest = new DeleteDecksRequest(new[] { uniqueDeckName }, true);
        var deleteDecksResponse = await _ankiService.DeleteDecksAsync(deleteDecksRequest);
        deleteDecksResponse.DeletedCount.Should().BeGreaterThanOrEqualTo(0);

        // Test 3: Verify the deck was actually deleted
        var finalGetDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest);
        finalGetDecksResponse.DeckNames.Should().NotContain(uniqueDeckName,
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
        var createDeckRequest = new CreateDeckRequest(uniqueDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest);
        _createdDecks.Add(uniqueDeckName);

        // Test CanAddNote with valid deck
        var testNote = new AnkiNote
        {
            DeckName = uniqueDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            },
            Tags = new List<string> { "integration-test" }
        };

        var canAddNoteRequest = new CanAddNoteRequest(testNote);
        var canAddNoteResponse = await _ankiService.CanAddNoteAsync(canAddNoteRequest);

        canAddNoteResponse.CanAdd.Should().BeTrue();
    }

    [Fact]
    public async Task AnkiService_CanAddNote_ShouldReturnTrue_WhenNoteIsValid()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        // Test CanAddNote with a valid note structure (even with non-existent deck)
        var testNote = new AnkiNote
        {
            DeckName = "NonExistentDeck_12345",
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            },
            Tags = new List<string> { "integration-test" }
        };

        var canAddNoteRequest = new CanAddNoteRequest(testNote);
        var canAddNoteResponse = await _ankiService.CanAddNoteAsync(canAddNoteRequest);

        canAddNoteResponse.CanAdd.Should().BeTrue();
    }

    [Fact]
    public async Task AnkiService_CreateNote_ShouldCreateNoteObject()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_CreateNote_{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequest(uniqueDeckName);
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
            },
            Tags = new List<string> { "integration-test" }
        };

        var createNoteRequest = new CreateNoteRequest(testNote);
        var createNoteResponse = await _ankiService.CreateNoteAsync(createNoteRequest);

        createNoteResponse.NoteId.Should().NotBeNull();
        createNoteResponse.NoteId.Should().BeGreaterThan(0);

        long createdNoteId = (long)createNoteResponse.NoteId!;

        // Verify note was created by finding it
        var findNotesRequest = new FindNotesRequest($"deck:{uniqueDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        findNotesResponse.NoteIds.Should().Contain(createdNoteId);

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
        var createDeckRequest = new CreateDeckRequest(uniqueDeckName);
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
            },
            Tags = new List<string> { "integration-test" }
        };

        var addNoteRequest = new AddNoteRequest(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);
        _createdNotes.Add(addNoteResponse.NoteId);

        // Update the note fields
        var updateFieldsRequest = new UpdateNoteFieldsRequest(addNoteResponse.NoteId, new Dictionary<string, string>
        {
            ["Back"] = UpdatedBack
        });
        var updateFieldsResponse = await _ankiService.UpdateNoteFieldsAsync(updateFieldsRequest);

        updateFieldsResponse.Success.Should().BeTrue();

        // Verify the note still exists (we can't easily verify field content without more complex API calls)
        var findNotesRequest = new FindNotesRequest($"deck:{uniqueDeckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest);

        findNotesResponse.NoteIds.Should().Contain(addNoteResponse.NoteId);
    }

    [Fact]
    public async Task AnkiService_AddTags_ShouldAddTagsToNotes()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_AddTags_{Guid.NewGuid():N}";
        var testTag1 = $"test-tag-1-{Guid.NewGuid():N}";
        var testTag2 = $"test-tag-2-{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequest(uniqueDeckName);
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
            },
            Tags = new List<string> { "integration-test" }
        };

        var addNoteRequest = new AddNoteRequest(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);
        _createdNotes.Add(addNoteResponse.NoteId);

        // Add tags to the note
        var addTagsRequest = new AddTagsRequest(new[] { addNoteResponse.NoteId }, new[] { testTag1, testTag2 });
        var addTagsResponse = await _ankiService.AddTagsAsync(addTagsRequest);

        addTagsResponse.Success.Should().BeTrue();

        // Verify tags were added by checking if they appear in the global tags list
        var getTagsRequest = new GetTagsRequest();
        var getTagsResponse = await _ankiService.GetTagsAsync(getTagsRequest);

        getTagsResponse.Tags.Should().Contain(testTag1);
        getTagsResponse.Tags.Should().Contain(testTag2);
    }

    [Fact]
    public async Task AnkiService_GetTags_ShouldReturnAllTags()
    {
        await EnsureAnkiConnectionAsync();
        await CaptureInitialDeckStateAsync();

        var uniqueDeckName = $"{TestDeckName}_GetTags_{Guid.NewGuid():N}";
        var testTag = $"test-tag-{Guid.NewGuid():N}";

        // Create deck first
        var createDeckRequest = new CreateDeckRequest(uniqueDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest);
        _createdDecks.Add(uniqueDeckName);

        // Create a note with a specific tag
        var testNote = new AnkiNote
        {
            DeckName = uniqueDeckName,
            ModelName = "Basic",
            Fields = new Dictionary<string, string>
            {
                ["Front"] = TestFront,
                ["Back"] = TestBack
            },
            Tags = new List<string> { "integration-test", testTag }
        };

        var addNoteRequest = new AddNoteRequest(testNote);
        var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest);
        _createdNotes.Add(addNoteResponse.NoteId);

        // Get all tags
        var getTagsRequest = new GetTagsRequest();
        var getTagsResponse = await _ankiService.GetTagsAsync(getTagsRequest);

        getTagsResponse.Tags.Should().NotBeNull();
        getTagsResponse.Tags.Should().Contain(testTag);
        getTagsResponse.Tags.Should().Contain("integration-test");
    }
}