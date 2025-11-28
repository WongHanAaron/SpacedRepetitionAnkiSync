using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for delete decks
/// </summary>
public record DeleteDecksResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public int? Result { get; init; }
}