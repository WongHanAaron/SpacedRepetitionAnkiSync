namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Request to find existing notes in Anki
/// </summary>
public record FindNotesRequest(string Query);