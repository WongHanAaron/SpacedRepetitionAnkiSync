using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to delete decks
/// </summary>
public record DeleteDecksRequestDto : AnkiConnectRequest
{
    public DeleteDecksRequestDto(IEnumerable<string> decks, bool cardsToo)
    {
        Action = "deleteDecks";
        Params = new DeleteDecksParams { Decks = decks, CardsToo = cardsToo };
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Parameters for delete decks request
/// </summary>
public record DeleteDecksParams
{
    [JsonPropertyName("decks")]
    public required IEnumerable<string> Decks { get; init; }

    [JsonPropertyName("cardsToo")]
    public bool CardsToo { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for delete decks
/// </summary>
public record DeleteDecksResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}