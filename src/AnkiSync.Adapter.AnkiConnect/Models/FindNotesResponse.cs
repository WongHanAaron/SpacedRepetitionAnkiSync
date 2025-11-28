using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for find notes
/// </summary>
public record FindNotesResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public List<long>? Result { get; init; }
}