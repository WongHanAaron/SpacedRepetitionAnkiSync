namespace AnkiSync.Domain.Core;

/// <summary>
/// Result of a synchronization operation
/// </summary>
public class SyncResult
{
    /// <summary>
    /// Whether the sync operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of flashcards processed
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Number of flashcards successfully synced
    /// </summary>
    public int SyncedCount { get; set; }

    /// <summary>
    /// Number of flashcards that failed to sync
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Details of failed operations
    /// </summary>
    public List<SyncError> Errors { get; set; } = new();

    /// <summary>
    /// When the sync operation completed
    /// </summary>
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Duration of the sync operation
    /// </summary>
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Current synchronization status
/// </summary>
public class SyncStatus
{
    /// <summary>
    /// Whether the sync service is currently running
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// Last sync operation result
    /// </summary>
    public SyncResult? LastSyncResult { get; set; }

    /// <summary>
    /// When the last sync operation occurred
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// Number of flashcards currently tracked
    /// </summary>
    public int TrackedFlashcardsCount { get; set; }

    /// <summary>
    /// Connection status to Anki
    /// </summary>
    public AnkiConnectionStatus AnkiConnectionStatus { get; set; }
}

/// <summary>
/// Anki connection status
/// </summary>
public enum AnkiConnectionStatus
{
    /// <summary>
    /// Connection not tested
    /// </summary>
    Unknown,

    /// <summary>
    /// Successfully connected to Anki
    /// </summary>
    Connected,

    /// <summary>
    /// Failed to connect to Anki
    /// </summary>
    Disconnected,

    /// <summary>
    /// Anki is not running
    /// </summary>
    AnkiNotRunning
}

/// <summary>
/// Details of a sync operation error
/// </summary>
public class SyncError
{
    /// <summary>
    /// The flashcard that failed
    /// </summary>
    public string FlashcardId { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error code or type
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// When the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}