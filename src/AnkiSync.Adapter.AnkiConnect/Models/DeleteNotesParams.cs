using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

public record DeleteNotesParams
{
    [JsonPropertyName("notes")]
    public required IEnumerable<long> Notes { get; init; }
}