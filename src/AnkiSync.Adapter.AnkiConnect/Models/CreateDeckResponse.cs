using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for create deck
/// </summary>
public record CreateDeckResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public long? Result { get; init; }
}