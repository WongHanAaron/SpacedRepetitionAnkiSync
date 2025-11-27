namespace AnkiSync.Domain.Interfaces;

/// <summary>
/// Primary port for flashcard synchronization operations
/// </summary>
public interface IAnkiSyncService
{
    /// <summary>
    /// Synchronizes flashcards from files to Anki
    /// </summary>
    /// <param name="flashcards">The flashcards to sync</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Results of the sync operation</returns>
    Task<SyncResult> SyncFlashcardsAsync(IEnumerable<Flashcard> flashcards, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current sync status
    /// </summary>
    /// <returns>Current synchronization status</returns>
    Task<SyncStatus> GetSyncStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that Anki connection is working
    /// </summary>
    /// <returns>True if Anki is accessible</returns>
    Task<bool> ValidateAnkiConnectionAsync(CancellationToken cancellationToken = default);
}