namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to get all decks
/// </summary>
public record GetDecksRequestDto : AnkiConnectRequest
{
    public GetDecksRequestDto()
    {
        Action = "deckNames";
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for deck names
/// </summary>
public record DeckNamesResponse : AnkiConnectResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("result")]
    public List<string>? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}