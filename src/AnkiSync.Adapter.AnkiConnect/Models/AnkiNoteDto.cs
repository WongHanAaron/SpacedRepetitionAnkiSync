namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Anki note DTO for serialization
/// </summary>
public record AnkiNoteDto
{
    public required string DeckName { get; init; }
    public required string ModelName { get; init; }
    public required Dictionary<string, string> Fields { get; init; }
    public List<string> Tags { get; init; } = new();
}