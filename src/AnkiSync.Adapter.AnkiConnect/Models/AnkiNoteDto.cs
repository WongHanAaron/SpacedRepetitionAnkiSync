using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Anki note DTO for serialization
/// </summary>
public record AnkiNoteDto
{
    [JsonPropertyName("deckName")]
    public required string DeckName { get; init; }

    [JsonPropertyName("modelName")]
    public required string ModelName { get; init; }

    [JsonPropertyName("fields")]
    public required Dictionary<string, string> Fields { get; init; }
}