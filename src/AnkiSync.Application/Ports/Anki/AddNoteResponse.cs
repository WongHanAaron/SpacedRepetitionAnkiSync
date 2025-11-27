namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response from adding a note to Anki
/// </summary>
public record AddNoteResponse(long NoteId);