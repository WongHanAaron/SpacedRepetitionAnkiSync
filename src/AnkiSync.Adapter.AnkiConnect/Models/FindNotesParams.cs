using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

public record FindNotesParams
{
    [JsonPropertyName("query")]
    public required string Query { get; init; }
}