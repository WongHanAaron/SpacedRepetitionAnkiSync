using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

public record CreateDeckParams
{
    [JsonPropertyName("deck")]
    public required string Deck { get; init; }
}