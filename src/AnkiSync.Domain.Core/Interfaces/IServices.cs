namespace AnkiSync.Domain.Core.Interfaces;

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

/// <summary>
/// Port for flashcard repository operations
/// </summary>
public interface IFlashcardRepository
{
    /// <summary>
    /// Gets all flashcards from the repository
    /// </summary>
    /// <returns>All flashcards</returns>
    Task<IEnumerable<Flashcard>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets flashcards by source file
    /// </summary>
    /// <param name="filePath">The source file path</param>
    /// <returns>Flashcards from the specified file</returns>
    Task<IEnumerable<Flashcard>> GetByFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates a flashcard
    /// </summary>
    /// <param name="flashcard">The flashcard to save</param>
    Task SaveAsync(Flashcard flashcard, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a flashcard by ID
    /// </summary>
    /// <param name="id">The flashcard ID</param>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Port for deck management operations
/// </summary>
public interface IDeckService
{
    /// <summary>
    /// Gets all available Anki decks
    /// </summary>
    /// <returns>List of deck names</returns>
    Task<IEnumerable<string>> GetDecksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a deck if it doesn't exist
    /// </summary>
    /// <param name="deckName">The deck name (supports hierarchy with ::)</param>
    Task CreateDeckAsync(string deckName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Infers the deck name from flashcard tags
    /// </summary>
    /// <param name="flashcard">The flashcard</param>
    /// <returns>The inferred deck name</returns>
    string InferDeck(Flashcard flashcard);
}