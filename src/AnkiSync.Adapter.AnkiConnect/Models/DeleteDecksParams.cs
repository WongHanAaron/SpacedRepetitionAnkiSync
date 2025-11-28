using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

public record DeleteDecksParams
{
    [JsonPropertyName("decks")]
    public required IEnumerable<string> Decks { get; init; }

    [JsonPropertyName("cardsToo")]
    public bool CardsToo { get; init; }
}