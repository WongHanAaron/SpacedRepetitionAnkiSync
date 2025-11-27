using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Configuration;
using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Domain.Core;
using AnkiSync.Domain.Core.Exceptions;
using AnkiSync.Domain.Core.Extensions;
using AnkiSync.Domain.Core.Interfaces;

namespace AnkiSync.Adapter.AnkiConnect;

/// <summary>
/// Main AnkiConnect client implementing domain interfaces
/// </summary>
public class AnkiConnectClient : IAnkiSyncService, IDeckService, IDisposable
{
    private readonly IAnkiConnectHttpClient _httpClient;
    private readonly AnkiConnectOptions _options;
    private bool _disposed;

    public AnkiConnectClient(AnkiConnectOptions? options = null)
    {
        _options = options ?? new AnkiConnectOptions();
        _httpClient = new AnkiConnectHttpClient(_options);
    }

    // For testing
    internal AnkiConnectClient(IAnkiConnectHttpClient httpClient, AnkiConnectOptions? options = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? new AnkiConnectOptions();
    }

    #region IAnkiSyncService Implementation

    public async Task<SyncResult> SyncFlashcardsAsync(IEnumerable<Flashcard> flashcards, CancellationToken cancellationToken = default)
    {
        var syncResult = new SyncResult
        {
            ProcessedCount = flashcards.Count(),
            CompletedAt = DateTime.UtcNow
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            foreach (var flashcard in flashcards)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await SyncSingleFlashcardAsync(flashcard, cancellationToken);
                    syncResult.SyncedCount++;
                }
                catch (Exception ex)
                {
                    syncResult.Errors.Add(new SyncError
                    {
                        FlashcardId = flashcard.Id,
                        Message = ex.Message,
                        ErrorCode = ex.GetType().Name,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            syncResult.Success = syncResult.Errors.Count == 0;
        }
        finally
        {
            stopwatch.Stop();
            syncResult.Duration = stopwatch.Elapsed;
        }

        syncResult.FailedCount = syncResult.Errors.Count;
        return syncResult;
    }

    public async Task<SyncStatus> GetSyncStatusAsync(CancellationToken cancellationToken = default)
    {
        // For now, just check connection status
        // In a full implementation, this would check sync state from database
        var isConnected = await ValidateAnkiConnectionAsync(cancellationToken);

        return new SyncStatus
        {
            IsRunning = false, // Not applicable for direct client
            AnkiConnectionStatus = isConnected
                ? AnkiConnectionStatus.Connected
                : AnkiConnectionStatus.Disconnected
        };
    }

    public async Task<bool> ValidateAnkiConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.TestConnectionAsync(cancellationToken);
    }

    #endregion

    #region IDeckService Implementation

    public async Task<IEnumerable<string>> GetDecksAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.InvokeAsync<List<string>>("deckNames", cancellationToken) ?? new List<string>();
    }

    public async Task CreateDeckAsync(string deckName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(deckName))
        {
            throw new ArgumentException("Deck name cannot be empty", nameof(deckName));
        }

        await _httpClient.InvokeAsync<long>("createDeck", new { deck = deckName }, cancellationToken);
    }

    public string InferDeck(Flashcard flashcard)
    {
        if (flashcard.Tags.Count == 0)
        {
            return "Default";
        }

        // Use the first tag and convert to Anki deck format
        var firstTag = flashcard.Tags[0];
        return firstTag.ToAnkiDeckName();
    }

    #endregion

    #region Private Methods

    private async Task SyncSingleFlashcardAsync(Flashcard flashcard, CancellationToken cancellationToken)
    {
        // Validate the flashcard first
        flashcard.Validate();

        // Infer the deck if not already set
        if (string.IsNullOrEmpty(flashcard.InferredDeck))
        {
            flashcard.InferredDeck = InferDeck(flashcard);
        }

        // Ensure the deck exists
        await CreateDeckAsync(flashcard.InferredDeck, cancellationToken);

        // Create the Anki note
        var note = CreateAnkiNote(flashcard);

        if (flashcard.IsSynced())
        {
            // Update existing note
            await _httpClient.InvokeAsync<object>("updateNoteFields", new
            {
                id = long.Parse(flashcard.AnkiNoteId!),
                fields = note.Fields
            }, cancellationToken);
        }
        else
        {
            // Create new note
            var result = await _httpClient.InvokeAsync<Dictionary<string, long>>("addNote", new
            {
                note = note
            }, cancellationToken);

            flashcard.AnkiNoteId = result["result"].ToString();
        }
    }

    private AnkiNote CreateAnkiNote(Flashcard flashcard)
    {
        // Map flashcard type to Anki model
        var modelName = flashcard.Type switch
        {
            FlashcardType.Basic => "Basic",
            FlashcardType.Bidirectional => "Basic (and reversed card)",
            FlashcardType.Cloze => "Cloze",
            _ => "Basic"
        };

        // Create fields based on model
        var fields = new Dictionary<string, string>();

        if (modelName.Contains("Cloze"))
        {
            fields["Text"] = $"{flashcard.Question}\n\n{flashcard.Answer}";
        }
        else
        {
            fields["Front"] = flashcard.Question;
            fields["Back"] = flashcard.Answer;
        }

        return new AnkiNote
        {
            ModelName = modelName,
            DeckName = flashcard.InferredDeck,
            Fields = fields,
            Tags = flashcard.Tags
        };
    }

    #endregion

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}