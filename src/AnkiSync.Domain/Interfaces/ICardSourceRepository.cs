using AnkiSync.Domain;

namespace AnkiSync.Domain.Interfaces;

/// <summary>
/// Repository for managing flashcard sources from external files
/// </summary>
public interface ICardSourceRepository
{
    /// <summary>
    /// Retrieves all flashcards from the specified file paths
    /// </summary>
    /// <param name="filePaths">The file paths to parse for flashcards</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of decks with their cards</returns>
    Task<IEnumerable<Deck>> GetCardsFromFiles(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all flashcards from files in the specified directories
    /// </summary>
    /// <param name="directories">The directories to scan for flashcard files</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of decks with their cards</returns>
    Task<IEnumerable<Deck>> GetCardsFromDirectories(IEnumerable<string> directories, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to changes in the specified directory and raises CardsUpdated event when files change
    /// </summary>
    /// <param name="directoryPath">The directory path to monitor for changes</param>
    void SubscribeToDirectoryChanges(string directoryPath);

    /// <summary>
    /// Event raised when cards in the sources might have been updated
    /// </summary>
    event EventHandler<CardsUpdatedEventArgs>? CardsUpdated;
}

/// <summary>
/// Event arguments for when cards are updated
/// </summary>
public class CardsUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// The file paths that were updated
    /// </summary>
    public IEnumerable<string> UpdatedFilePaths { get; }

    /// <summary>
    /// Initializes a new instance of CardsUpdatedEventArgs
    /// </summary>
    /// <param name="updatedFilePaths">The file paths that were updated</param>
    public CardsUpdatedEventArgs(IEnumerable<string> updatedFilePaths)
    {
        UpdatedFilePaths = updatedFilePaths;
    }
}