namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response for creating a note object
/// </summary>
public record CreateNoteResponse(long? NoteId);