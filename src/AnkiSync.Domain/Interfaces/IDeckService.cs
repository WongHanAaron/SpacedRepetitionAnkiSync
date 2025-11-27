namespace AnkiSync.Domain.Interfaces;

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