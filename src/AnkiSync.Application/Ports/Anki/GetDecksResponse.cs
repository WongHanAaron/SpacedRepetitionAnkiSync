namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response containing available decks from Anki
/// </summary>
public record GetDecksResponse(IEnumerable<string> DeckNames);