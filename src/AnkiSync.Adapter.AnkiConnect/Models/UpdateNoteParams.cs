using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

public record UpdateNoteParams
{
    [JsonPropertyName("note")]
    public required UpdateNoteData Note { get; init; }
}

public record UpdateNoteData
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("fields")]
    public required Dictionary<string, string> Fields { get; init; }
}