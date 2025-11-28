namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to delete decks from Anki
/// </summary>
public record DeleteDecksRequest(IEnumerable<string> DeckNames, bool CardsToo);