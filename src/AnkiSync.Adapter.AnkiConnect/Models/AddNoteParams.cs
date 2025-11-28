using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

public record AddNoteParams
{
    [JsonPropertyName("note")]
    public required AnkiNoteDto Note { get; init; }
}