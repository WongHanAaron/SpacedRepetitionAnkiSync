using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for createNote
/// </summary>
public record CreateNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public long? Result { get; init; }
}