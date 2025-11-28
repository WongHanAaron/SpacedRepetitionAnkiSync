namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to delete notes from Anki
/// </summary>
public record DeleteNotesRequest(IEnumerable<long> NoteIds);