using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for update note
/// </summary>
public record UpdateNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
}