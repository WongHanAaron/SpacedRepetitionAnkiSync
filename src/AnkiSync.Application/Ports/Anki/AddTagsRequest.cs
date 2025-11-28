namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to add or remove tags from notes
/// </summary>
public record AddTagsRequest(IEnumerable<long> NoteIds, IEnumerable<string> Tags, bool Add = true);