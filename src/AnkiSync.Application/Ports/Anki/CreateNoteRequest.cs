using AnkiSync.Domain;

namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to create a note object (without saving it)
/// </summary>
public record CreateNoteRequest(AnkiNote Note);