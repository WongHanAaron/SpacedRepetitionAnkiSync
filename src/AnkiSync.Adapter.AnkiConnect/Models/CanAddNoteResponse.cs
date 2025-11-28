using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for canAddNote
/// </summary>
public record CanAddNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
}