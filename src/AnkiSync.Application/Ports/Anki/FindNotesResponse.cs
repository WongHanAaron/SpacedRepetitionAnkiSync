namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response containing found note IDs from Anki
/// </summary>
public record FindNotesResponse(IEnumerable<long> NoteIds);