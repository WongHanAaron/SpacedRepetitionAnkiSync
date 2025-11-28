namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response for checking if a note can be added
/// </summary>
public record CanAddNoteResponse(bool CanAdd);