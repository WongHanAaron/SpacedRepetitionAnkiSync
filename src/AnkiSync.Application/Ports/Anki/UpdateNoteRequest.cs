namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to update an existing note in Anki
/// </summary>
public record UpdateNoteRequest(long NoteId, Dictionary<string, string> Fields);