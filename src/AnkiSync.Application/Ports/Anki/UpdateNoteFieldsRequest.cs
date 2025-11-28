namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to update fields of an existing note
/// </summary>
public record UpdateNoteFieldsRequest(long NoteId, Dictionary<string, string> Fields);