namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to create a deck in Anki
/// </summary>
public record CreateDeckRequest(string DeckName);