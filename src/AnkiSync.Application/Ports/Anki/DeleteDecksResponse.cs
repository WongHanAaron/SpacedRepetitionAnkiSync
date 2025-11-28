namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response from deleting decks from Anki
/// </summary>
public record DeleteDecksResponse(int DeletedCount);