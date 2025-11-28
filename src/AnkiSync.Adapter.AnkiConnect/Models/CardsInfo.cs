using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to get detailed information about cards
/// </summary>
public record CardsInfoRequestDto : AnkiConnectRequest
{
    public CardsInfoRequestDto(IEnumerable<long> cards)
    {
        Action = "cardsInfo";
        Params = new CardsInfoParams { Cards = cards };
    }
}

/// <summary>
/// Parameters for cardsInfo request
/// </summary>
public record CardsInfoParams
{
    [JsonPropertyName("cards")]
    public required IEnumerable<long> Cards { get; init; }
}

/// <summary>
/// Response for cardsInfo
/// </summary>
public record CardsInfoResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public List<CardInfo>? Result { get; init; }
}

/// <summary>
/// Information about a single card
/// </summary>
public record CardInfo
{
    [JsonPropertyName("cardId")]
    public long CardId { get; init; }

    [JsonPropertyName("note")]
    public long Note { get; init; }

    [JsonPropertyName("ord")]
    public int Ordinal { get; init; }

    [JsonPropertyName("type")]
    public int Type { get; init; }

    [JsonPropertyName("queue")]
    public int Queue { get; init; }

    [JsonPropertyName("due")]
    public long Due { get; init; }

    [JsonPropertyName("ivl")]
    public int Interval { get; init; }

    [JsonPropertyName("factor")]
    public int Factor { get; init; }

    [JsonPropertyName("reps")]
    public int Reps { get; init; }

    [JsonPropertyName("lapses")]
    public int Lapses { get; init; }

    [JsonPropertyName("left")]
    public int Left { get; init; }

    [JsonPropertyName("odue")]
    public long OriginalDue { get; init; }

    [JsonPropertyName("odid")]
    public long OriginalDeckId { get; init; }

    [JsonPropertyName("flags")]
    public int Flags { get; init; }

    [JsonPropertyName("data")]
    public string Data { get; init; } = string.Empty;
}