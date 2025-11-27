namespace AnkiSync.Domain;

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