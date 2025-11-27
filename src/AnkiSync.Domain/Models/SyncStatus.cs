namespace AnkiSync.Domain;

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