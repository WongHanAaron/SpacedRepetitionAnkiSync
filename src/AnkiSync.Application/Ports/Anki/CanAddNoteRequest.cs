using AnkiSync.Domain;

namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to check if a note can be added to Anki
/// </summary>
public record CanAddNoteRequest(AnkiNote Note);