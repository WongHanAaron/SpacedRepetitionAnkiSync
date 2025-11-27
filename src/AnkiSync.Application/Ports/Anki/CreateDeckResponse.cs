namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response from creating a deck in Anki
/// </summary>
public record CreateDeckResponse(bool Success);