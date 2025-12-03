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
    /// Gets all decks that exist in Anki with their cards
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all decks in Anki with their cards</returns>
    Task<IEnumerable<Deck>> GetAllDecksAsync(CancellationToken cancellationToken = default);

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