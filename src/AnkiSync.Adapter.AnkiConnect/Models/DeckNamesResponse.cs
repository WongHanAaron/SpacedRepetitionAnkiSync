using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for deck names
/// </summary>
public record DeckNamesResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public List<string>? Result { get; init; }
}