using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to create a deck
/// </summary>
public record CreateDeckRequestDto : AnkiConnectRequest
{
    public CreateDeckRequestDto(string deck)
    {
        Action = "createDeck";
        Params = new CreateDeckParams { Deck = deck };
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Parameters for create deck request
/// </summary>
public record CreateDeckParams
{
    [JsonPropertyName("deck")]
    public required string Deck { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for create deck
/// </summary>
public record CreateDeckResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public long? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}