using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to change the deck of cards
/// </summary>
public record ChangeDeckRequestDto : AnkiConnectRequest
{
    public ChangeDeckRequestDto(IEnumerable<long> cards, string deck)
    {
        Action = "changeDeck";
        Params = new ChangeDeckParams { Cards = cards, Deck = deck };
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Parameters for changeDeck request
/// </summary>
public record ChangeDeckParams
{
    [JsonPropertyName("cards")]
    public required IEnumerable<long> Cards { get; init; }

    [JsonPropertyName("deck")]
    public required string Deck { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for changeDeck
/// </summary>
public record ChangeDeckResponse : AnkiConnectResponse
{
    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}