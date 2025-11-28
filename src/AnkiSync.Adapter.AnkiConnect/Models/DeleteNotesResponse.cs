using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for delete notes
/// </summary>
public record DeleteNotesResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public int? Result { get; init; }
}