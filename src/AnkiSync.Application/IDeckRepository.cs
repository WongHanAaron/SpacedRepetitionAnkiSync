using AnkiSync.Domain;

namespace AnkiSync.Application;

/// <summary>
/// Repository for managing deck operations with Anki
/// </summary>
public interface IDeckRepository
{
    /// <summary>
    /// Downloads a deck by name from Anki and converts it to domain models
    /// </summary>
    /// <param name="deckName">The name of the deck to download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deck with all its cards</returns>
    Task<Deck> DownloadDeckAsync(string deckName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a deck to Anki, creating it if it doesn't exist
    /// </summary>
    /// <param name="deck">The deck to upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The uploaded deck identifier</returns>
    Task<DeckIdentifier> UploadDeckAsync(Deck deck, CancellationToken cancellationToken = default);
}