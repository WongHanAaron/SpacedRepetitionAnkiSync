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
}

/// <summary>
/// Response for deck names
/// </summary>
public record DeckNamesResponse : AnkiConnectResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("result")]
    public List<string>? Result { get; init; }
}