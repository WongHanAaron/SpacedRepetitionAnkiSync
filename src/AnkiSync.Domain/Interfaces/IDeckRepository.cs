using AnkiSync.Domain;
using AnkiSync.Domain.Models;

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
    /// <returns>The deck with all its cards, or null if the deck doesn't exist</returns>
    Task<Deck?> GetDeck(DeckId deckId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a deck to Anki, creating or updating it as necessary
    /// </summary>
    /// <param name="deck">The deck to upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpsertDeck(Deck deck, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all deck IDs that exist in Anki
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all deck IDs in Anki</returns>
    Task<IEnumerable<DeckId>> GetAllDeckIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a deck from Anki
    /// </summary>
    /// <param name="deckId">The identifier of the deck to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteDeckAsync(DeckId deckId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes cards from a deck that are no longer in the source
    /// </summary>
    /// <param name="deckId">The deck identifier</param>
    /// <param name="cardsToKeep">The cards that should remain in the deck</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteObsoleteCardsAsync(DeckId deckId, IEnumerable<Card> cardsToKeep, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a collection of synchronization instructions
    /// </summary>
    /// <param name="instructions">The instructions to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExecuteInstructionsAsync(IEnumerable<SynchronizationInstruction> instructions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Syncs the Anki collection with AnkiWeb
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SyncWithAnkiWebAsync(CancellationToken cancellationToken = default);
}