using AnkiSync.Domain;

namespace AnkiSync.Domain;

/// <summary>
/// Repository for managing deck operations with Anki
/// </summary>
public interface IDeckRepository
{
    /// <summary>
    /// Downloads a deck by identifier from Anki and converts it to domain models
    /// </summary>
    /// <param name="deckId">The identifier of the deck to download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deck with all its cards</returns>
    Task<Deck> GetDeck(DeckId deckId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a deck to Anki, creating or updating it as necessary
    /// </summary>
    /// <param name="deck">The deck to upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpsertDeck(Deck deck, CancellationToken cancellationToken = default);
}