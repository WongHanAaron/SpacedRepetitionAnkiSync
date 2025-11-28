using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for add note
/// </summary>
public record AddNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public long? Result { get; init; }
}