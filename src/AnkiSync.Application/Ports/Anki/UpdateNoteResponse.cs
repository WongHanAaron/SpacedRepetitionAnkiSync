namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response from updating a note in Anki
/// </summary>
public record UpdateNoteResponse(bool Success);