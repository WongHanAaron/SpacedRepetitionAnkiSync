using AnkiSync.Domain;

namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to add a note to Anki
/// </summary>
public record AddNoteRequest(AnkiNote Note);