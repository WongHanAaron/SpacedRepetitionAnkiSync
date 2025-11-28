namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response from deleting notes from Anki
/// </summary>
public record DeleteNotesResponse(int DeletedCount);