using AnkiSync.Domain;

namespace AnkiSync.Application;

/// <summary>
/// Application service for importing decks from Anki
/// </summary>
public interface IDeckImportService
{
    /// <summary>
    /// Downloads a deck by name from Anki and converts it to domain models
    /// </summary>
    /// <param name="deckName">The name of the deck to download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deck with all its cards</returns>
    Task<Deck> DownloadDeckAsync(string deckName, CancellationToken cancellationToken = default);
}